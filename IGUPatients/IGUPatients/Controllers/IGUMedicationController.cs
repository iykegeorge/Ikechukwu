using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IGUPatients.Models;
using Microsoft.AspNetCore.Http;

namespace IGUPatients.Controllers
{
    public class IGUMedicationController : Controller
    {
        private readonly PatientsContext _context;

        public IGUMedicationController(PatientsContext context)
        {
            _context = context;
        }
        // Function for listing the Medication
        // GET: IGUMedication
        public async Task<IActionResult> Index(string id, string name)
        {
            if(String.IsNullOrWhiteSpace(id) || String.IsNullOrWhiteSpace(name))
            {
                if (String.IsNullOrEmpty(HttpContext.Session.GetString("code")))
                {
                    TempData["Error"] = "No medication type code available";

                    return RedirectToAction("Index", "VKMedicationType");

                }
                else
                {
                    id = HttpContext.Session.GetString("code");
                    name = HttpContext.Session.GetString("medName");

                }
            }
            else
            {
                HttpContext.Session.SetString("code", id);
                HttpContext.Session.SetString("medName", name);
            }
            if (String.IsNullOrWhiteSpace(name))
            {
                name = _context.MedicationType.FirstOrDefault(c => c.MedicationTypeId == Convert.ToInt32(id)).Name;
            }
            ViewData["Name"] = name;
            ViewData["medName"] = name;

            var patientsContext = _context.Medication.Include(m => m.ConcentrationCodeNavigation)
                .Include(m => m.DispensingCodeNavigation).Include(m => m.MedicationType)
                .Where(m => m.MedicationTypeId.ToString() == id)
                .OrderBy(m => m.Name).ThenBy(m => m.Concentration);

            return View(await patientsContext.ToListAsync());
        }
        // Function to view the details
        // GET: IGUMedication/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medication
                .Include(m => m.ConcentrationCodeNavigation)
                .Include(m => m.DispensingCodeNavigation)
                .Include(m => m.MedicationType)
                .FirstOrDefaultAsync(m => m.Din == id);
            if (medication == null)
            {
                return NotFound();
            }
            ViewData["medName"] = HttpContext.Session.GetString("medNmae");
            return View(medication);
        }

        // Function for creating Medication
        // GET: IGUMedication/Create
        public IActionResult Create()
        {
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit.OrderBy(m => m.ConcentrationCode), "ConcentrationCode", "ConcentrationCode");
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit.OrderBy(m => m.DispensingCode), "DispensingCode", "DispensingCode");
            ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name");
            ViewData["medName"] = HttpContext.Session.GetString("medName");
            return View();
        }
        // Function for saving the created medication
        // POST: IGUMedication/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Din,Name,Image,MedicationTypeId,DispensingCode,Concentration,ConcentrationCode")] Medication medication)
        {
            medication.MedicationTypeId = Convert.ToInt32(HttpContext.Session.GetString("code"));
            if (_context.Medication.Any(m => m.Name == medication.Name) && _context.Medication.Any(m => m.Concentration == medication.Concentration) && _context.Medication.Any(m => m.ConcentrationCode == medication.ConcentrationCode))
            {
                ModelState.AddModelError("Name", "Medication already exists");
            }

            if (ModelState.IsValid)
            {
                _context.Add(medication);
                await _context.SaveChangesAsync();
                TempData["AlertMessage"] = "Record created";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit.OrderBy(m => m.ConcentrationCode), "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit.OrderBy(m => m.DispensingCode), "DispensingCode", "DispensingCode", medication.DispensingCode);
            ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name", medication.MedicationTypeId);
            ViewData["medName"] = HttpContext.Session.GetString("medName");
            return View(medication);
        }

        // Funtion for opening the edit page
        // GET: IGUMedication/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medication.FindAsync(id);
            if (medication == null)
            {
                return NotFound();
            }
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit, "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit, "DispensingCode", "DispensingCode", medication.DispensingCode);
            ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name", medication.MedicationTypeId);
            ViewData["medName"] = HttpContext.Session.GetString("medName");
            return View(medication);
        }

        // Function for saving the editted file
        // POST: IGUMedication/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Din,Name,Image,MedicationTypeId,DispensingCode,Concentration,ConcentrationCode")] Medication medication)
        {
            if (id != medication.Din)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicationExists(medication.Din))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["AlertMessage"] = "Record edited";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit, "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit, "DispensingCode", "DispensingCode", medication.DispensingCode);
            ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name", medication.MedicationTypeId);
            ViewData["medName"] = HttpContext.Session.GetString("medName");


            return View(medication);
        }
        // Function for opening the delete page
        // GET: IGUMedication/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medication
                .Include(m => m.ConcentrationCodeNavigation)
                .Include(m => m.DispensingCodeNavigation)
                .Include(m => m.MedicationType)
                .FirstOrDefaultAsync(m => m.Din == id);
            if (medication == null)
            {
                return NotFound();
            }
            ViewData["medName"] = HttpContext.Session.GetString("medName");
            return View(medication);
        }
        // Function for saving the delete changes
        // POST: IGUMedication/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var medication = await _context.Medication.Include(m => m.PatientMedication).Include(m => m.TreatmentMedication).FirstOrDefaultAsync(m => m.Din == id);
            _context.Medication.Remove(medication);
            await _context.SaveChangesAsync();
            ViewData["medName"] = HttpContext.Session.GetString("medName");
            TempData["AlertMessage"] = "Record deleted";
            return RedirectToAction(nameof(Index));
        }
        //This function checks for dublication
        private bool MedicationExists(string id)
        {
            return _context.Medication.Any(e => e.Din == id);
        }
    }
}
