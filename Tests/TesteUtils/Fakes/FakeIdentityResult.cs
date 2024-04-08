using Microsoft.AspNetCore.Identity;

namespace Tests.TesteUtils.Fakes;

public class FakeIdentityResult : IdentityResult
{
	public FakeIdentityResult(bool succeeded)
	{
		Succeeded = succeeded;
	}
}