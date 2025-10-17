using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using System;
using System.Linq;
using System.Threading;
using System.Data.SqlClient;
using Xunit;

namespace E2ETest;
public class KasaE2E : IDisposable
{
    private readonly string appPath = @"C:\Users\Korisnik\Documents\GitHub\MaloprodajniTestiranje\MaloprodajniObjekatCs-main\MaloprodajniObjekat\bin\Debug\MaloprodajniObjekat.exe";
    private readonly string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Korisnik\Documents\GitHub\MaloprodajniTestiranje\MaloprodajniObjekatCs-main\MaloprodajniObjekat\Database\MaloprodajniObjekat.mdf;Integrated Security=True";
    private Application app;

    private UIA3Automation automation;

    public KasaE2E()
    {

    }

    [Fact]
    public void PurchaseE2E()
    {
        app = Application.Launch(appPath);
        automation = new UIA3Automation();

        var loginWindow = app.GetMainWindow(automation);
        Assert.NotNull(loginWindow);

        var usernameBox = WaitForElement(() => loginWindow.FindFirstDescendant(cf => cf.ByAutomationId("usernameBox"))?.AsTextBox(), 3000);
        var passwordBox = WaitForElement(() => loginWindow.FindFirstDescendant(cf => cf.ByAutomationId("passwordBox"))?.AsTextBox(), 3000);

        Assert.NotNull(usernameBox);
        Assert.NotNull(passwordBox);

        usernameBox.Enter("maxpetrasov");
        passwordBox.Enter("maxpetras03");

        var kasirLoginBtn = WaitForElement(() => loginWindow.FindFirstDescendant(cf => cf.ByText("Uloguj se kao KASIR"))?.AsButton(), 3000);
        Assert.NotNull(kasirLoginBtn);
        kasirLoginBtn.Invoke();

        var KasirMeni = RetryFindTopLevelWindow(app, automation, titleContains: "KASIR MENI", timeoutMs: 5000);
        Assert.NotNull(KasirMeni);

        var kupovinaBtn = WaitForElement(() => KasirMeni.FindFirstDescendant(cf => cf.ByText("KUPOVINA"))?.AsButton(), 3000);
        Assert.NotNull(kupovinaBtn);
        kupovinaBtn.Invoke();

        var kupovinaWindow = RetryFindTopLevelWindow(app, automation, titleContains: "Kupovina", timeoutMs: 4000);
        var uiRoot = kupovinaWindow ?? KasirMeni;

        var barkodBox = WaitForElement(() => uiRoot.FindFirstDescendant(cf => cf.ByAutomationId("barkod"))?.AsTextBox(), 3000);
        var kolicinaBox = WaitForElement(() => uiRoot.FindFirstDescendant(cf => cf.ByAutomationId("kolicina"))?.AsTextBox(), 3000);
        var artikliPrikaz = WaitForElement(() => uiRoot.FindFirstDescendant(cf => cf.ByAutomationId("artikliPrikaz"))?.AsDataGridView(), 5000);

        Assert.NotNull(barkodBox);
        Assert.NotNull(kolicinaBox);
        Assert.NotNull(artikliPrikaz);

        var foundRow = FindRowByCellValue(artikliPrikaz, "artikalID", "4");
        if (foundRow != null)
        {
            if (foundRow is FlaUI.Core.AutomationElements.DataGridViewRow dataRow)
            {
                dataRow.Click();
                dataRow.Patterns.SelectionItem.Pattern.Select();
            }
            else
            {
                foundRow.Click();
                var cell = foundRow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem));
                cell?.Click();
            }
        }
        kolicinaBox.Enter("8");
        var dodajUKorpuBtn = WaitForElement(() => uiRoot.FindFirstDescendant(cf => cf.ByText("DODAJ U KORPU"))?.AsButton(), 2000);
        Assert.NotNull(dodajUKorpuBtn);
        dodajUKorpuBtn.Invoke();

        var zavrsiKupovinuBtn = WaitForElement(() => uiRoot.FindFirstDescendant(cf => cf.ByText("ZAVRŠI KUPOVINU"))?.AsButton(), 2000);
        Assert.NotNull(zavrsiKupovinuBtn);
        zavrsiKupovinuBtn.Invoke();

        var PotvrdaKupovine = RetryFindTopLevelWindow(app, automation, titleContains: "Potvrdi kupovinu?", timeoutMs: 4000);
        Assert.NotNull(PotvrdaKupovine);

        var yesBtn = WaitForElement(() => PotvrdaKupovine.FindFirstDescendant(cf => cf.ByText("POTVRDI"))?.AsButton(), 2000);
        Assert.NotNull(yesBtn);
        yesBtn.Invoke();
        CloseAnyMessageBox(app, automation, timeoutMs: 2000);
        Thread.Sleep(800);

        var racuniBtn = WaitForElement(() => KasirMeni.FindFirstDescendant(cf => cf.ByText("RAČUNI"))?.AsButton(), 3000);
        Assert.NotNull(racuniBtn);
        racuniBtn.Invoke();

        var RacuniWindow = RetryFindTopLevelWindow(app, automation, titleContains: "Racuni", timeoutMs: 4000);
        var uiRootRacuni = RacuniWindow ?? KasirMeni;

        var racDataGrid = WaitForElement(() => uiRootRacuni.FindFirstDescendant(cf => cf.ByAutomationId("racDataGrid"))?.AsDataGridView(), 5000);
        var prikaziRacunBtn = WaitForElement(() => uiRootRacuni.FindFirstDescendant(cf => cf.ByAutomationId("prikazRacunaBtn"))?.AsButton(), 3000);
        var racunTextBox = WaitForElement(() => uiRootRacuni.FindFirstDescendant(cf => cf.ByAutomationId("racunPrikaz"))?.AsTextBox(), 3000);
        var racunIDTxt = WaitForElement(() => uiRootRacuni.FindFirstDescendant(cf => cf.ByAutomationId("racunIDTxt"))?.AsTextBox(), 3000);

        Assert.NotNull(racDataGrid);
        Assert.NotNull(prikaziRacunBtn);
        Assert.NotNull(racunTextBox);
        Assert.NotNull(racunIDTxt);

        int foundID;
        string lastRacunID;

        using (var connection = new SqlConnection(connString))
        {
            connection.Open();
            using (var command = new SqlCommand("SELECT TOP 1 racunID FROM Racun ORDER BY racunID DESC", connection))
            {
                foundID = (int)command.ExecuteScalar();
            }
        }

        lastRacunID = foundID.ToString();
        racunIDTxt.Enter(lastRacunID);
        prikaziRacunBtn.Invoke();
        Thread.Sleep(4000);
    }

    private static Window RetryFindTopLevelWindow(Application app, UIA3Automation automation, string titleContains, int timeoutMs)
    {
        var end = DateTime.Now.AddMilliseconds(timeoutMs);
        while (DateTime.Now < end)
        {
            var win = app.GetAllTopLevelWindows(automation).FirstOrDefault(w => w.Title?.IndexOf(titleContains, StringComparison.OrdinalIgnoreCase) >= 0);
            if (win != null) return win;
            Thread.Sleep(200);
        }
        return null;
    }

    private static T WaitForElement<T>(Func<T> factory, int timeoutMs) where T : class
    {
        var end = DateTime.Now.AddMilliseconds(timeoutMs);
        while (DateTime.Now < end)
        {
            var el = factory();
            if (el != null) return el;
            Thread.Sleep(150);
        }
        return null;
    }

    private static AutomationElement FindRowByCellValue(DataGridView grid, object columnIndexOrName, string cellText)
    {
        if (grid == null) return null;
        foreach (var row in grid.Rows)
        {
            foreach (var cell in row.Cells)
            {
                try
                {
                    var val = cell.Value?.ToString();
                    if (string.Equals(val, cellText, StringComparison.OrdinalIgnoreCase))
                    {
                        return row;
                    }
                }
                catch { }
            }
        }
        return null;
    }

    private static void CloseAnyMessageBox(Application app, UIA3Automation automation, int timeoutMs)
    {
        var end = DateTime.Now.AddMilliseconds(timeoutMs);
        while (DateTime.Now < end)
        {
            var topWins = app.GetAllTopLevelWindows(automation);
            foreach (var w in topWins)
            {
                // skip main app windows that are too large - message boxes usually have short titles
                var ok = w.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton()
                      ?? w.FindFirstDescendant(cf => cf.ByText("Ok"))?.AsButton()
                      ?? w.FindFirstDescendant(cf => cf.ByText("U redu"))?.AsButton()
                      ?? w.FindFirstDescendant(cf => cf.ByText("Uredu"))?.AsButton();

                if (ok != null)
                {
                    try { ok.Invoke(); return; }
                    catch { }
                }
            }
            Thread.Sleep(100);
        }
    }

    public void Dispose()
    {
        try
        {
            automation?.Dispose();
            if (app != null && !app.HasExited)
            {
                app.Close();
                app.Dispose();
            }
        }
        catch { }
    }
}
