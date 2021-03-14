using System.Threading.Tasks;
using FluentAssertions;
using IdentityService.Client;
using IdentityService.Grpc;
using IdentityService.Migrations;
using Microsoft.AspNetCore.Identity;
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
				Password = "Qwerty-Qwerty1",
			};

			static void SeedData(ApplicationDbContext context)
			{
				var existingUser = new IdentityUser
				{
					Id = "4d4081ee710c499ab0061fdf9b1be841",
					UserName = "SomeUser@test.com",
					NormalizedUserName = "SOMEUSER@TEST.COM",
					Email = "SomeUser@test.com",
					NormalizedEmail = "SOMEUSER@TEST.COM",
					EmailConfirmed = false,
					PasswordHash = "AQAAAAEAACcQAAAAEC8/kY4JPWgfNPTXw6b9K0oPtCn6NcX8+nw2Q0PaOlwsPhwSHh2Ltz1z81FhJVBGrQ==",
					SecurityStamp = "KOCXTBU5L72Q6GTBORBRVVXEDUENTHOM",
					ConcurrencyStamp = "b9a1a2bf-0f91-405f-b63e-3434ae0a5795",
					PhoneNumber = null,
					PhoneNumberConfirmed = false,
					TwoFactorEnabled = false,
					LockoutEnd = null,
					LockoutEnabled = true,
					AccessFailedCount = 0,
				};

				context.Users.Add(existingUser);
			}

			using var factory = new CustomWebApplicationFactory(SeedData);
			var client = factory.CreateServiceClient<IIdentityServiceClient>();

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
	}
}
