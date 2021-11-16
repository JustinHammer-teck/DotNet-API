using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DotNet_API.Contracts.V1;
using DotNet_API.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace DotNet_API.IntegrationTests
{
    public class PostControllerTest : IntegrationTest
    {
        [Fact]
        public async Task GetAll_WithoutAnyPost_ReturnEmptyRespone()
        {
            //Arrange
            
            //Act
            var respone = await TestClient.GetAsync(ApiRoutes.Posts.GetAll);
            //Assert
            respone.StatusCode.Should().Be(HttpStatusCode.OK);

        }
    }
}