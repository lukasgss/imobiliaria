using Microsoft.AspNetCore.Identity;

namespace Tests.TesteUtils.Fakes;

public class FakeSignInResult : SignInResult
{
	public FakeSignInResult(bool succeeded, bool isLockedOut)
	{
		Succeeded = succeeded;
		IsLockedOut = isLockedOut;
	}
}