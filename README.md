# Gerenciamento de imobiliária

## Funcionamento do sistema

Foi pedido que fizesse um sistema no âmbito de locação de imóveis. A partir disso, a forma como o sistema foi projetado para funcionar é a seguinte:
1. Usuário se registra ou faz login na plataforma;
2. Usuário cadastra um imóvel no qual pretende locar (usuário que o registra é sempre atribuído como o dono);
3. Usuário cadastra uma locação, especificando qual o imóvel, o locatário, a data de vencimento e o valor mensal da locação;
4. Usuário que cadastrou a locação a assina (o sistema trata locações como um contrato);
5. Usuário locatário que foi especificado na locação cadastrada anteriormente também assina a locação;
6. Com isso, a locação foi assinada por ambas as partes e é preenchido o valor na data de fechamento como a data atual, indicando quando a locação foi assinada por ambas as partes.


## Arquitetura do sistema

O sistema foi feito baseado na Clean Architecture e foi dividido em 4 camadas, sendo elas: Aplicação, Infraestrutura, Domínio e Api.

### Aplicação

É a camada responsável pela lógica de negócios da aplicação. Ela realiza validações, mapeia dados entre entidades de domínio e DTOs ou vice-versa, controla o fluxo de execução e faz chamadas à camada de Infraestrutura para obter dados de serviços externos como APIs de terceiros ou realizar acesso ao banco de dados. Além disso, é onde as interfaces são definidas, fazendo com que a camada de Infraestrutura dependa dela, e não o contrário.

### Domínio

É o core da aplicação, onde todas as regras de negócio são definidas. Ela contém as entidades de domínio e a lógica de negócios que operam sobre essas entidades. Por motivos de simplicidade da aplicação, não foi tão necessária uma abordagem mais DDD, fazendo com que essa aplicação sofra do que é chamado de um domínio anêmico, como descrito pelo Martin Fowler nesse [artigo](https://martinfowler.com/bliki/AnemicDomainModel.html). Este é um domínio no qual não possui comportamento e não encapsula as regras de negócio, apenas define as entidades.

### Infraestrutura

Fornece as implementações de acesso a serviços externos, como acesso a bancos de dados, chamadas a APIs externas, etc.

### Api

Essa camada é responsável por expor as funcionalidades da aplicação para os usuários. Ela trata de roteamento de solicitações, serialização/desserialização de dados e interação com a camada de Aplicação, chamando seus serviços. É o entrypoint da aplicação e onde as controllers são definidas.


Assim fica o gráfico de dependências entre os projetos da solução:


![image](https://github.com/lukasgss/imobiliaria/assets/69154977/67b0ea9f-e996-4338-a6c9-f30feae188d1)


## Testes unitários

Os testes unitários foram feitos utilizando XUnit e NSubstitute para o mock de dependências. Foram utilizadas classes geradoras de dados, como por exemplo a classe GeradorImovel e classes de constantes para cada entidade, mantendo os valores padronizados.
Para rodar os testes, basta executar o seguinte comando na pasta raíz do projeto:
```bash
dotnet test
```

## Boas práticas no projeto

- Transactions onde são atualizadas múltiplas entidades, garantindo a [atomicidade](https://en.wikipedia.org/wiki/Atomicity_(database_systems)) da operação, assegurando que não haja inconsistências nos valores no banco;
-  O projeto foi separado em camadas, cada uma com sua responsabilidade única, mantendo o sistema de fácil manutenção e suscetível a futuras alterações;
-  Padrões de projeto definidos, onde as interfaces são majoritariamente definidas na camada de Aplicação, por exemplo. Isso possibilita com que seja utilizada a camada de Infraestrutura na camada de Aplicação sem com que a camada de Aplicação dependa da de Infraestrutura;
-  Adição de testes unitários, facilitando a manutenção e garantindo o funcionamento correto da aplicação;
-  Princípios SOLID;
-  Clean Code;
  

##  Rodar o projeto

Para rodar o projeto, primeiramente é necessário executar as migrations para a criação do banco de dados. Para isso, entre na pasta de Infraestrutura e execute o seguinte comando:
```bash
dotnet ef --startup-project ../Api/Api.csproj database update
```
Com isso, basta rodar o projeto com o seguinte comando:
```bash
dotnet run
```
Ao visitar a página no swagger, é possível interagir com todos os endpoints e testar a aplicação.

## Estrutura de logs

Foi utilizado [Serilog](https://serilog.net/) juntamente com [Seq](https://datalust.co/seq) como alternativa para armazenamento de logs. É uma alternativa self-hosted para logs estruturados e é possível configurá-lo com Docker facilmente com as instruções na [documentação](https://docs.datalust.co/docs/getting-started-with-docker).
As configurações do Serilog estão no arquivo de appSettings, considerando que o Seq foi configurado utilizando Docker e da exata forma como especificada na documentação. Caso tenha configurado de maneira diferente, é necessário ajustar os valores no arquivo de configuração.


![image](https://github.com/lukasgss/imobiliaria/assets/69154977/6091d901-3d22-4e4b-9766-b300cc531684)


Com o Seq configurado no Docker, basta entrar na aplicação web disponibilizada na porta especificada (8081 no meu caso) e analisar os logs registrados:


![image](https://github.com/lukasgss/imobiliaria/assets/69154977/f2ade7e3-c218-4429-8444-2c63386ba552)


## Observações
1. Para inserir o token JWT no swagger, não é necessário adicionar o prefixo de Bearer, basta adicionar o token retornado pelo endpoint;
2. Os arquivos de configurações que possuem dados sensíveis foram adicionados ao repositório propositalmente com o intuito de facilitar a criação do banco, dos serviços de logs e dos testes da aplicação.
