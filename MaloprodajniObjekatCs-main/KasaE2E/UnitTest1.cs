using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using System;
using System.IO;
using Xunit;

namespace KasaE2E
{
    public class KasirKupovinaE2ETests : IDisposable
    {
        private readonly Application _app;
        private readonly AutomationBase _automation;
        private readonly Window _mainWindow;

        public KasirKupovinaE2ETests()
        {
            // Path to your WPF app executable
            var appPath = Path.Combine(Environment.CurrentDirectory, @"..\..\..\MaloprodajniObjekat\bin\Debug\net6.0-windows\MaloprodajniObjekat.exe");
            _app = Application.Launch(appPath);
            _automation = new UIA3Automation();
            _mainWindow = _app.GetMainWindow(_automation);
        }

        [Fact]
        public void AddItemToCart_And_CompletePurchase()
        {
            // 1. Navigate to KasirKupovina page (assume a button with automation id "KasirKupovinaBtn")
            var kasirBtn = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("KasirKupovinaBtn"))?.AsButton();
            Assert.NotNull(kasirBtn);
            kasirBtn.Invoke();

            // 2. Wait for KasirKupovina page to load (assume label with automation id "ukupnaCenaLbl")
            var ukupnaCenaLbl = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ukupnaCenaLbl"))?.AsLabel();
            Assert.NotNull(ukupnaCenaLbl);

            // 3. Select an item from the inventory (assume DataGrid with automation id "artikliPrikaz")
            var artikliPrikaz = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("artikliPrikaz"))?.AsDataGridView();
            Assert.NotNull(artikliPrikaz);
            var firstRow = artikliPrikaz.Rows[0];
            firstRow.Click();

            // 4. Set quantity (assume TextBox with automation id "kolicina")
            var kolicinaBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("kolicina"))?.AsTextBox();
            Assert.NotNull(kolicinaBox);
            kolicinaBox.Text = "2";

            // 5. Click "Add to Cart" (assume Button with automation id "dodajUKorpuBtn")
            var addToCartBtn = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("dodajUKorpuBtn"))?.AsButton();
            Assert.NotNull(addToCartBtn);
            addToCartBtn.Invoke();

            // 6. Assert cart total is updated
            ukupnaCenaLbl = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ukupnaCenaLbl"))?.AsLabel();
            Assert.NotEqual("0", ukupnaCenaLbl.Text);

            // 7. Complete purchase (assume Button with automation id "zavrsiKupovinuBtn")
            var finishBtn = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("zavrsiKupovinuBtn"))?.AsButton();
            Assert.NotNull(finishBtn);
            finishBtn.Invoke();

            // 8. Assert confirmation dialog appears (assume Window with title "PotvrdaKupovine")
            var confirmation = _mainWindow.ModalWindows.FirstOrDefault(w => w.Title.Contains("PotvrdaKupovine"));
            Assert.NotNull(confirmation);
        }

        public void Dispose()
        {
            _automation.Dispose();
            _app.Close();
        }
    }
}