using System.Data;
using NUnit.Framework;
using Moq;
using Dapper;
using Microsoft.EntityFrameworkCore;


namespace ORM
{
    [TestFixture]
    public class Tests
    {
        private RepositoryDapper _repositoryDapper;
        private RepositoryADO _repositoryAdo;
        private RepositoryADODisconnected _repositoryADODisconnected;
        private RepositoryEFCore _repositoryEFCore;
        
        private Mock<IDbConnection> _connectionMock;

        [SetUp]
        public void Setup()
        {
            _connectionMock = new Mock<IDbConnection>();
            _repositoryDapper = new RepositoryDapper(_connectionMock.Object.ToString());
        }

        [Test]
        public void AddProduct_ShouldAddProductToDatabaseDapper()
        {
            var product = new Product
            {
                Description = "Test Product",
                Weight = 10,
                Height = 5,
                Width = 3,
                Length = 7
            };

            _connectionMock.Setup(conn => conn.Execute(It.IsAny<string>(), product, null, null, null)).Returns(1);

            _repositoryDapper.AddProduct(product);

            _connectionMock.Verify(conn => conn.Execute(It.IsAny<string>(), product, null, null, null), Times.Once);
        }

        [Test]
        public void AddProduct_ShouldAddProductToDatabaseADO()
        {
            var product = new Product
            {
                Description = "Test Product",
                Weight = 10,
                Height = 5,
                Width = 3,
                Length = 7
            };

            _connectionMock.Setup(conn => conn.Execute(It.IsAny<string>(), product, null, null, null)).Returns(1);

            _repositoryAdo.AddProduct(product);

            _connectionMock.Verify(conn => conn.Execute(It.IsAny<string>(), product, null, null, null), Times.Once);
        }

        [Test]
        public void AddProduct_ShouldAddProductToDatabaseADODisconnected()
        {
            var product = new Product
            {
                Description = "Test Product",
                Weight = 10,
                Height = 5,
                Width = 3,
                Length = 7
            };

            _connectionMock.Setup(conn => conn.Execute(It.IsAny<string>(), product, null, null, null)).Returns(1);

            _repositoryADODisconnected.AddProduct(product);

            _connectionMock.Verify(conn => conn.Execute(It.IsAny<string>(), product, null, null, null), Times.Once);
        }

        [Test]
        public void AddProduct_ShouldAddProductToDatabaseEFCore()
        {
            var product = new Product
            {
                Description = "Test Product",
                Weight = 10,
                Height = 5,
                Width = 3,
                Length = 7
            };

            _connectionMock.Setup(conn => conn.Execute(It.IsAny<string>(), product, null, null, null)).Returns(1);

            _repositoryEFCore.AddProduct(product);

            _connectionMock.Verify(conn => conn.Execute(It.IsAny<string>(), product, null, null, null), Times.Once);
        }
    }
}

