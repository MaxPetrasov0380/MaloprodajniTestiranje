//\Users\bvkib\source\repos\Teretana\Teretana\obj\DebugTeretana.exe

using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using System;
using System.Linq;
using System.Threading;
using Xunit;

namespace E2ETest;
public class OpremaE2E_CRUDTests : IDisposable
{
    private readonly string appPath = @"C:\Users\bvkib\source\repos\Teretana\Teretana\bin\Debug\Teretana.exe";
    private Application app;

    private UIA3Automation automation;

    public OpremaE2E_CRUDTests()
    {
        // Nothing here yet
    }

    [Fact]
    public void Full_Crud_E2E_Through_UI()
    {
        // Start app
        app = Application.Launch(appPath);
        automation = new UIA3Automation();

        // Login window
        var loginWindow = app.GetMainWindow(automation);
        Assert.NotNull(loginWindow);

        // Enter credentials ('username' and 'password')
        var usernameBox = WaitForElement(() => loginWindow.FindFirstDescendant(cf => cf.ByAutomationId("user"))?.AsTextBox(), 3000);
        var passwordBox = WaitForElement(() => loginWindow.FindFirstDescendant(cf => cf.ByAutomationId("pass"))?.AsTextBox(), 3000);

        Assert.NotNull(usernameBox);
        Assert.NotNull(passwordBox);

        usernameBox.Enter("bojan");
        passwordBox.Enter("bojan123");

        // Click Admin button (by text)
        var adminBtn = WaitForElement(() => loginWindow.FindFirstDescendant(cf => cf.ByText("Admin"))?.AsButton(), 3000);
        Assert.NotNull(adminBtn);
        adminBtn.Invoke();

        // Wait for Admin menu window (or main window title changes)
        var AdminMeni = RetryFindTopLevelWindow(app, automation, titleContains: "Admin", timeoutMs: 5000);
        Assert.NotNull(AdminMeni);

        // Click Oprema button in admin menu (by text)
        var opremaBtn = WaitForElement(() => AdminMeni.FindFirstDescendant(cf => cf.ByText("OPREMA"))?.AsButton(), 3000);
        Assert.NotNull(opremaBtn);
        opremaBtn.Invoke();

        // Wait for page or control with ODataGrid to be available
        var opremaWindow = RetryFindTopLevelWindow(app, automation, titleContains: "Oprema", timeoutMs: 4000);
        // If page is inside same window, fallback to adminWindow
        var uiRoot = opremaWindow ?? AdminMeni;

        // Wait for controls defined in your XAML by Name: naziv, opis, max, datum, kolicina, lokacija, ODataGrid
        var nazivBox = WaitForElement(() => uiRoot.FindFirstDescendant(cf => cf.ByAutomationId("naziv"))?.AsTextBox(), 3000);
        var opisBox = WaitForElement(() => uiRoot.FindFirstDescendant(cf => cf.ByAutomationId("opis"))?.AsTextBox(), 3000);
        var maxBox = WaitForElement(() => uiRoot.FindFirstDescendant(cf => cf.ByAutomationId("max"))?.AsTextBox(), 3000);
        var datumPicker = WaitForElement(() => uiRoot.FindFirstDescendant(cf => cf.ByAutomationId("datum")), 3000); // DatePicker is not be TextBox
        var kolicinaBox = WaitForElement(() => uiRoot.FindFirstDescendant(cf => cf.ByAutomationId("kolicina"))?.AsTextBox(), 3000);
        var lokacijaCombo = WaitForElement(() => uiRoot.FindFirstDescendant(cf => cf.ByAutomationId("lokacija"))?.AsComboBox(), 3000);
        var dataGrid = WaitForElement(() => uiRoot.FindFirstDescendant(cf => cf.ByAutomationId("ODataGrid"))?.AsDataGridView(), 5000);

        Assert.NotNull(nazivBox);
        Assert.NotNull(opisBox);
        Assert.NotNull(maxBox);
        Assert.NotNull(kolicinaBox);
        Assert.NotNull(lokacijaCombo);
        Assert.NotNull(dataGrid);

        // Prepare unique name so test identifies row reliably
        string baseName = "E2E_TEST_OPREMA_" + Guid.NewGuid().ToString("N").Substring(0, 6);
        string updatedName = baseName + "_EDIT";

        // --- ADD ---
        nazivBox.Enter(baseName);
        opisBox.Enter("E2E opis");
        maxBox.Enter("123");

        try
        {
            var dp = datumPicker.AsDateTimePicker();
            if (dp != null)
            {
                dp.SelectedDate = DateTime.Today;
            }
            else
            {
                // fallback: try to find a textbox inside
                var dpText = datumPicker.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit))?.AsTextBox();
                dpText?.Enter(DateTime.Today.ToShortDateString());
            }
        }
        catch { /* ignore; best-effort */ }

        kolicinaBox.Enter("2");

        // Select first item in lokacija combo 
        if (lokacijaCombo.Items.Length == 0)
        {
            throw new InvalidOperationException("ComboBox 'lokacija' nema itema. Dodaj bar jednu lokaciju u bazu pre pokretanja E2E testa.");
        }
        lokacijaCombo.Select(0);

        // Click Dodaj button (by text)
        var dodajBtn = WaitForElement(() => uiRoot.FindFirstDescendant(cf => cf.ByText("Dodaj"))?.AsButton(), 2000);
        Assert.NotNull(dodajBtn);
        dodajBtn.Invoke();

        // Close possible MessageBox (OK)
        CloseAnyMessageBox(app, automation, timeoutMs: 2000);

        // Wait a bit to allow DataGrid refresh
        Thread.Sleep(800);

        // --- VERIFY ADDED ---
        dataGrid = WaitForElement(() => uiRoot.FindFirstDescendant(cf => cf.ByAutomationId("ODataGrid"))?.AsDataGridView(), 3000);
        var addedRow = FindRowByCellValue(dataGrid, columnIndexOrName: "NazivO", cellText: baseName);
        Assert.NotNull(addedRow);

        // --- EDIT ---
        // Select the row
        var selectionPattern = addedRow.Patterns.SelectionItem.Pattern;
        selectionPattern.Select();

        // Fill new name (put updatedName into naziv textbox)
        nazivBox = WaitForElement(() => uiRoot.FindFirstDescendant(cf => cf.ByAutomationId("naziv"))?.AsTextBox(), 1000);
        Assert.NotNull(nazivBox);
        nazivBox.Enter(updatedName);

        var izmeniBtn = WaitForElement(() => uiRoot.FindFirstDescendant(cf => cf.ByText("Izmeni"))?.AsButton(), 2000);
        Assert.NotNull(izmeniBtn);
        izmeniBtn.Invoke();

        // Close message box if any
        CloseAnyMessageBox(app, automation, timeoutMs: 2000);
        Thread.Sleep(800);

        // Verify update
        dataGrid = WaitForElement(() => uiRoot.FindFirstDescendant(cf => cf.ByAutomationId("ODataGrid"))?.AsDataGridView(), 3000);
        var editedRow = FindRowByCellValue(dataGrid, columnIndexOrName: "NazivO", cellText: updatedName);
        Assert.NotNull(editedRow);

        // --- DELETE ---
        editedRow.Patterns.SelectionItem.Pattern.Select();
        var obrisiBtn = WaitForElement(() => uiRoot.FindFirstDescendant(cf => cf.ByText("Obrisi"))?.AsButton(), 2000);
        Assert.NotNull(obrisiBtn);
        obrisiBtn.Invoke();

        // Close message box if any
        CloseAnyMessageBox(app, automation, timeoutMs: 2000);
        Thread.Sleep(800);

        // Verify deletion
        dataGrid = WaitForElement(() => uiRoot.FindFirstDescendant(cf => cf.ByAutomationId("ODataGrid"))?.AsDataGridView(), 3000);
        var deletedRow = FindRowByCellValue(dataGrid, columnIndexOrName: "NazivO", cellText: updatedName);
        Assert.Null(deletedRow);
    }

    // Helper: retry find top-level window containing title fragment
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

    // Helper: generic waiter for element factory
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

    // Helper: find DataGrid row by column name or index and text
    private static AutomationElement FindRowByCellValue(DataGridView grid, object columnIndexOrName, string cellText)
    {
        if (grid == null) return null;
        foreach (var row in grid.Rows)
        {
            // Try find by name: search cells for matching text
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

    // Helper: closes modal MessageBox windows with OK/U redu button
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
