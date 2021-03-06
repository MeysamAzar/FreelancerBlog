﻿//using System.Collections.Generic;
//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;
//using FluentAssertions;
//using FreelancerBlog.Core.Types;
//using FreelancerBlog.Services.Shared;
//using FreelancerBlog.UnitTests.HandMadeFakes;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Primitives;
//using Moq;
//using Xunit;

//namespace FreelancerBlog.UnitTests.Services.Shared
//{
//    public class CaptchaValidatorTests
//    {
//        private Mock<IHttpContextAccessor> _contextAccessor;
//        private Mock<FakeHttpMessageHandler> _fakeHttpMessageHandler;
//        private HttpClient _httpClient;
//        public CaptchaValidatorTests()
//        {
//            _contextAccessor = new Mock<IHttpContextAccessor>();
//            _fakeHttpMessageHandler = new Mock<FakeHttpMessageHandler> { CallBase = true };
//            _httpClient = new HttpClient(_fakeHttpMessageHandler.Object);
//        }

//        [Fact]
//        public async Task ValidateCaptchaAsync_ShouldReturn_TheCorrectType()
//        {
//            //Arrange
//            var httpContext = new DefaultHttpContext();

//            _contextAccessor.SetupGet(c => c.HttpContext).Returns(httpContext);
//            _contextAccessor.SetupGet(c => c.HttpContext.Request.Form)
//                .Returns(new FormCollection(new Dictionary<string, StringValues>()));

//            _fakeHttpMessageHandler.Setup(f => f.Send(It.IsAny<HttpRequestMessage>())).Returns(new HttpResponseMessage
//            {
//                StatusCode = HttpStatusCode.OK,
//                Content =
//                    new StringContent(
//                        "{\"success\": false,\"error-codes\": [\"It's a fake error!\",\"It's a fake error\"]}")
//            });

//            var sut = new CaptchaValidator(_contextAccessor.Object, _httpClient);

//            //Act
//            var result = await sut.ValidateCaptchaAsync("dummy-secret");

//            //Assert
//            result.Should().NotBeNull();
//            result.Should().BeOfType<CaptchaResponse>();
//        }


//_captchaValidator.Setup(c => c.ValidateCaptchaAsync(_configurationWrapper.Object.GetValue<string>("secrect")))
//.ReturnsAsync(new CaptchaResponse
//{
//ChallengeTimeStamp = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
//ErrorCodes = new List<string> { },
//HostName = "localhost",
//Success = "true"
//});

//        [Fact]
//        public async Task ValidateCaptchaAsync_ShouldReturnSuccessFalse_IfResponseSuccessWasFalse()
//        {
//            //Arrange
//            var httpContext = new DefaultHttpContext();

//            _contextAccessor.SetupGet(c => c.HttpContext).Returns(httpContext);
//            _contextAccessor.SetupGet(c => c.HttpContext.Request.Form)
//                .Returns(new FormCollection(new Dictionary<string, StringValues>()));

//            _fakeHttpMessageHandler.Setup(f => f.Send(It.IsAny<HttpRequestMessage>())).Returns(new HttpResponseMessage
//            {
//                StatusCode = HttpStatusCode.OK,
//                Content =
//                    new StringContent(
//                        "{\"success\": false,\"error-codes\": [\"It's a fake error!\",\"It's a fake error\"]}")
//            });

//            var sut = new CaptchaValidator(_contextAccessor.Object, _httpClient);

//            //Act
//            var result = await sut.ValidateCaptchaAsync("dummy-secret");

//            //Assert
//            result.Should().NotBeNull();
//            result.Success.Should().Be("false");
//        }



//        [Fact]
//        public async Task ValidateCaptchaAsync_ShouldReturnSuccessTrue_IfResponseSuccessWasTrue()
//        {
//            //Arrange
//            var httpContext = new DefaultHttpContext();

//            _contextAccessor.SetupGet(c => c.HttpContext).Returns(httpContext);
//            _contextAccessor.SetupGet(c => c.HttpContext.Request.Form)
//                .Returns(new FormCollection(new Dictionary<string, StringValues>()));

//            _fakeHttpMessageHandler.Setup(f => f.Send(It.IsAny<HttpRequestMessage>())).Returns(new HttpResponseMessage
//            {
//                StatusCode = HttpStatusCode.OK,
//                Content =
//                    new StringContent(
//                        "{\"success\": true,\"error-codes\": [\"It's a fake error!\",\"It's a fake error\"]}")
//            });

//            var sut = new CaptchaValidator(_contextAccessor.Object, _httpClient);

//            //Act
//            var result = await sut.ValidateCaptchaAsync("dummy-secret");

//            //Assert
//            result.Should().NotBeNull();
//            result.Success.Should().Be("true");
//        }


//    }
//}

////_captchaValidator.Setup(c => c.ValidateCaptchaAsync(_configurationWrapper.Object.GetValue<string>("secrect")))
////.ReturnsAsync(new CaptchaResponse
////{
////ChallengeTimeStamp = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
////ErrorCodes = new List<string> { },
////HostName = "localhost",
////Success = "true"
////});