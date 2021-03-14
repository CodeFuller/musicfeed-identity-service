using System.Threading.Tasks;
using FluentAssertions;
using IdentityService.Client;
using IdentityService.Grpc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IdentityService.IntegrationTests
{
	[TestClass]
	public class IdentityServiceTests
	{
		[TestMethod]
		public async Task RegisterUser_ForCorrectUserData_CreatesUserSuccessfully()
		{
			// Arrange

			var request = new RegisterUserRequest
			{
				Email = "SomeUser@test.com",
				Password = "Qwerty-Qwerty1",
			};

			using var factory = new CustomWebApplicationFactory();
			var client = factory.CreateServiceClient<IIdentityServiceClient>();

			// Act

			var response = await client.RegisterUserAsync(request);

			// Assert

			response.UserId.Should().NotBeNullOrWhiteSpace();
			response.Errors.Should().BeEmpty();
		}

		[TestMethod]
		public async Task RegisterUser_IfEmailIsInvalid_ReturnsError()
		{
			// Arrange

			var request = new RegisterUserRequest
			{
				Email = "SomeUser",
				Password = "Qwerty-Qwerty1",
			};

			using var factory = new CustomWebApplicationFactory();
			var client = factory.CreateServiceClient<IIdentityServiceClient>();

			// Act

			var response = await client.RegisterUserAsync(request);

			// Assert

			var expectedError = new IdentityServiceError
			{
				ErrorCode = "InvalidEmail",
				ErrorDescription = "Email 'SomeUser' is invalid.",
			};

			response.UserId.Should().BeEmpty();
			response.Errors.Should().BeEquivalentTo(expectedError);
		}

		[TestMethod]
		public async Task RegisterUser_IfUserWithSameLoginAlreadyExists_ReturnsError()
		{
			// Arrange

			var request = new RegisterUserRequest
			{
				Email = "SoMeUsEr@TeSt.CoM",
				Password = "QwErTy-QwErTy1",
			};

			using var factory = new CustomWebApplicationFactory();
			var client = factory.CreateServiceClient<IIdentityServiceClient>();

			await SeedUser(client, "SomeUser@test.com", "Qwerty-Qwerty1");

			// Act

			var response = await client.RegisterUserAsync(request);

			// Assert

			var expectedError = new IdentityServiceError
			{
				ErrorCode = "DuplicateUserName",
				ErrorDescription = "Username 'SoMeUsEr@TeSt.CoM' is already taken.",
			};

			response.UserId.Should().BeEmpty();
			response.Errors.Should().BeEquivalentTo(expectedError);
		}

		[TestMethod]
		public async Task RegisterUser_ForPasswordWithoutDigit_ReturnsError()
		{
			var expectedError = new IdentityServiceError
			{
				ErrorCode = "PasswordRequiresDigit",
				ErrorDescription = "Passwords must have at least one digit ('0'-'9').",
			};

			await WeakPasswordTestCase("Qwerty-Qwerty", expectedError);
		}

		[TestMethod]
		public async Task RegisterUser_ForPasswordWithoutLowercaseCharacter_ReturnsError()
		{
			var expectedError = new IdentityServiceError
			{
				ErrorCode = "PasswordRequiresLower",
				ErrorDescription = "Passwords must have at least one lowercase ('a'-'z').",
			};

			await WeakPasswordTestCase("QWERTY-QWERTY1", expectedError);
		}

		[TestMethod]
		public async Task RegisterUser_ForPasswordWithoutNonAlphanumericCharacter_ReturnsError()
		{
			var expectedError = new IdentityServiceError
			{
				ErrorCode = "PasswordRequiresNonAlphanumeric",
				ErrorDescription = "Passwords must have at least one non alphanumeric character.",
			};

			await WeakPasswordTestCase("QwertyQwerty1", expectedError);
		}

		[TestMethod]
		public async Task RegisterUser_ForPasswordWithoutUppercaseCharacter_ReturnsError()
		{
			var expectedError = new IdentityServiceError
			{
				ErrorCode = "PasswordRequiresUpper",
				ErrorDescription = "Passwords must have at least one uppercase ('A'-'Z').",
			};

			await WeakPasswordTestCase("qwerty-qwerty1", expectedError);
		}

		[TestMethod]
		public async Task RegisterUser_ForPasswordWhichIsTooShort_ReturnsError()
		{
			var expectedError = new IdentityServiceError
			{
				ErrorCode = "PasswordTooShort",
				ErrorDescription = "Passwords must be at least 8 characters.",
			};

			await WeakPasswordTestCase("Some-Q1", expectedError);
		}

		[TestMethod]
		public async Task RegisterUser_ForPasswordWithNotEnoughUniqueChars_ReturnsError()
		{
			var expectedError = new IdentityServiceError
			{
				ErrorCode = "PasswordRequiresUniqueChars",
				ErrorDescription = "Passwords must use at least 5 different characters.",
			};

			await WeakPasswordTestCase("Qqq-Qqq1", expectedError);
		}

		private static async Task WeakPasswordTestCase(string password, IdentityServiceError expectedError)
		{
			// Arrange

			var request = new RegisterUserRequest
			{
				Email = "SomeUser@test.com",
				Password = password,
			};

			using var factory = new CustomWebApplicationFactory();
			var client = factory.CreateServiceClient<IIdentityServiceClient>();

			// Act

			var response = await client.RegisterUserAsync(request);

			// Assert

			response.UserId.Should().BeEmpty();
			response.Errors.Should().BeEquivalentTo(expectedError);
		}

		[TestMethod]
		public async Task CheckUser_ForCorrectUserCredentials_CompletesSuccessfully()
		{
			// Arrange

			var request = new CheckUserRequest
			{
				Email = "SomeUser@test.com",
				Password = "Qwerty-Qwerty1",
			};

			using var factory = new CustomWebApplicationFactory();
			var client = factory.CreateServiceClient<IIdentityServiceClient>();

			await SeedUser(client, "SomeUser@test.com", "Qwerty-Qwerty1");

			// Act

			var response = await client.CheckUserAsync(request);

			// Assert

			response.UserId.Should().NotBeNullOrWhiteSpace();
			response.Errors.Should().BeEmpty();
		}

		[TestMethod]
		public async Task CheckUser_ForEmailInDifferentCase_CompletesSuccessfully()
		{
			// Arrange

			var request = new CheckUserRequest
			{
				Email = "SoMeUsEr@TeSt.CoM",
				Password = "Qwerty-Qwerty1",
			};

			using var factory = new CustomWebApplicationFactory();
			var client = factory.CreateServiceClient<IIdentityServiceClient>();

			await SeedUser(client, "SomeUser@test.com", "Qwerty-Qwerty1");

			// Act

			var response = await client.CheckUserAsync(request);

			// Assert

			response.UserId.Should().NotBeNullOrWhiteSpace();
			response.Errors.Should().BeEmpty();
		}

		[TestMethod]
		public async Task CheckUser_ForCorrectCredentialsAfterFourFailedAttempts_CompletesSuccessfully()
		{
			// Arrange

			var correctRequest = new CheckUserRequest
			{
				Email = "SomeUser@test.com",
				Password = "Qwerty-Qwerty1",
			};

			using var factory = new CustomWebApplicationFactory();
			var client = factory.CreateServiceClient<IIdentityServiceClient>();

			await SeedUser(client, "SomeUser@test.com", "Qwerty-Qwerty1");

			async Task CheckUserWithIncorrectPassword()
			{
				var incorrectRequest = new CheckUserRequest
				{
					Email = "SomeUser@test.com",
					Password = "Qwerty-Qwerty",
				};

				var failedResponse = await client.CheckUserAsync(incorrectRequest);

				// Sanity check
				failedResponse.UserId.Should().BeEmpty();
				failedResponse.Errors.Should().NotBeEmpty();
			}

			await CheckUserWithIncorrectPassword();
			await CheckUserWithIncorrectPassword();
			await CheckUserWithIncorrectPassword();
			await CheckUserWithIncorrectPassword();

			// Act

			var response = await client.CheckUserAsync(correctRequest);

			// Assert

			response.UserId.Should().NotBeNullOrWhiteSpace();
			response.Errors.Should().BeEmpty();
		}

		[TestMethod]
		public async Task CheckUser_ForCorrectCredentialsAfterFiveFailedAttempts_ReturnsError()
		{
			// Arrange

			var correctRequest = new CheckUserRequest
			{
				Email = "SomeUser@test.com",
				Password = "Qwerty-Qwerty1",
			};

			using var factory = new CustomWebApplicationFactory();
			var client = factory.CreateServiceClient<IIdentityServiceClient>();

			await SeedUser(client, "SomeUser@test.com", "Qwerty-Qwerty1");

			async Task CheckUserWithIncorrectPassword()
			{
				var incorrectRequest = new CheckUserRequest
				{
					Email = "SomeUser@test.com",
					Password = "Qwerty-Qwerty",
				};

				var failedResponse = await client.CheckUserAsync(incorrectRequest);

				// Sanity check
				failedResponse.UserId.Should().BeEmpty();
				failedResponse.Errors.Should().NotBeEmpty();
			}

			await CheckUserWithIncorrectPassword();
			await CheckUserWithIncorrectPassword();
			await CheckUserWithIncorrectPassword();
			await CheckUserWithIncorrectPassword();
			await CheckUserWithIncorrectPassword();

			// Act

			var response = await client.CheckUserAsync(correctRequest);

			// Assert

			var expectedError = new IdentityServiceError
			{
				ErrorCode = "IncorrectUserNameOrPassword",
				ErrorDescription = "The user name or password is incorrect.",
			};

			response.UserId.Should().BeEmpty();
			response.Errors.Should().BeEquivalentTo(expectedError);
		}

		[TestMethod]
		public async Task CheckUser_ForIncorrectUserName_ReturnsError()
		{
			// Arrange

			var request = new CheckUserRequest
			{
				Email = "AnotherUser@test.com",
				Password = "Qwerty-Qwerty1",
			};

			using var factory = new CustomWebApplicationFactory();
			var client = factory.CreateServiceClient<IIdentityServiceClient>();

			await SeedUser(client, "SomeUser@test.com", "Qwerty-Qwerty1");

			// Act

			var response = await client.CheckUserAsync(request);

			// Assert

			var expectedError = new IdentityServiceError
			{
				ErrorCode = "IncorrectUserNameOrPassword",
				ErrorDescription = "The user name or password is incorrect.",
			};

			response.UserId.Should().BeEmpty();
			response.Errors.Should().BeEquivalentTo(expectedError);
		}

		[TestMethod]
		public async Task CheckUser_ForIncorrectPassword_ReturnsError()
		{
			// Arrange

			var request = new CheckUserRequest
			{
				Email = "SomeUser@test.com",
				Password = "Qwerty-Qwerty",
			};

			using var factory = new CustomWebApplicationFactory();
			var client = factory.CreateServiceClient<IIdentityServiceClient>();

			await SeedUser(client, "SomeUser@test.com", "Qwerty-Qwerty1");

			// Act

			var response = await client.CheckUserAsync(request);

			// Assert

			var expectedError = new IdentityServiceError
			{
				ErrorCode = "IncorrectUserNameOrPassword",
				ErrorDescription = "The user name or password is incorrect.",
			};

			response.UserId.Should().BeEmpty();
			response.Errors.Should().BeEquivalentTo(expectedError);
		}

		private static async Task SeedUser(IIdentityServiceClient client, string userName, string password)
		{
			var request = new RegisterUserRequest
			{
				Email = userName,
				Password = password,
			};

			var seedResponse = await client.RegisterUserAsync(request);

			// Sanity check
			seedResponse.UserId.Should().NotBeNullOrWhiteSpace();
			seedResponse.Errors.Should().BeEmpty();
		}
	}
}
