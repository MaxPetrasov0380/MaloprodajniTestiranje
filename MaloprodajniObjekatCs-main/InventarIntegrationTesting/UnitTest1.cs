using System;
using Xunit;
using MaloprodajniObjekat.Servisi;

namespace InventarIntegrationTesting
{
    public class InventarServisIntegrationTests
    {
        private readonly string _testConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Korisnik\\Documents\\GitHub\\MaloprodajniTestiranje\\MaloprodajniObjekatCs-main\\MaloprodajniObjekat\\Database\\MaloprodajniObjekat.mdf;Integrated Security=True";
        [Fact]
        public void Create_Update_Delete_IntegrationTest()
        {
            var repo = new ArtikliRepository(_testConnectionString);
            var servis = new ArtikliServis(repo);

            bool created = servis.create("XIXO Tutti Frutti","Bezalkoholno pice",60,100);
            Assert.True(created, "Neuspesno dodavanje testa");

            string id = repo.getLastId();
            Assert.False(string.IsNullOrEmpty(id), "Neuspesno dobijanje ID-a novododatog testa");

            bool updated = servis.update(int.Parse(id), "XIXO Tutti Frutti","Bezalkoholno pice",70,150);
            Assert.True(updated, "Neuspesno azuriranje zapisa");

            bool deleted = servis.delete(int.Parse(id));
            Assert.True(deleted, "Neuspesno brisanje zapisa");
        }
    }
}