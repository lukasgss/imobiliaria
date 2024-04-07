using Application.Common.Interfaces.Providers;

namespace Application.Common.Providers;

public class ValueProvider : IValueProvider
{
	public DateTime UtcNow() => DateTime.Now;
}