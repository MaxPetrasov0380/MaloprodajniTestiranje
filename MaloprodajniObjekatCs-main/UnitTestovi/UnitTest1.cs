using Moq;
using MaloprodajniObjekat.Servisi;

namespace UnitTesting;

public class UnitTest1
{
    [Fact]
    public void create_ValidateData()
    {
        var mock = new Mock<IArtikliRepository>();
        mock.Setup(r => r.create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(true);
        var servis = new ArtikliServis(mock.Object);
        var result = servis.create("Blanc Pivo 0.5l", "Alkoholno pice", 120, 65);
        Assert.True(result);
        mock.Verify(r => r.create("Blanc Pivo 0.5l", "Alkoholno pice", 120, 65), Times.Once);
    }

    [Fact]
    public void update_Exists() 
    {
        var mock = new Mock<IArtikliRepository>();
        mock.Setup(r => r.update(20,"Boom Energy 0.33l","Bezalkoholno pice",60,122)).Returns(true);
        var servis = new ArtikliServis(mock.Object);
        var result = servis.update(20, "Boom Energy 0.33l", "Bezalkoholno pice", 60, 122);
        Assert.True(result);
        mock.Verify(r => r.update(20, "Boom Energy 0.33l", "Bezalkoholno pice", 60, 122), Times.Once);
    }

    [Fact]
    public void delete_Exists()
    {
        var mock = new Mock<IArtikliRepository>();
        mock.Setup(r => r.delete(20)).Returns(true);
        var servis = new ArtikliServis(mock.Object);
        var result = servis.delete(20);
        Assert.True(result);
        mock.Verify(r => r.delete(20), Times.Once);
    }
}