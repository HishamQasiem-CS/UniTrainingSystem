﻿namespace UniTrainingSystem.VIewModels
{
    public class EditCompanyViewModel
    {
        public int CompanyId { get; set; }

        public string CompanyName { get; set; }
        public string Email { get; set; }
        public int PhoneNumber { get; set; }
        public string Description { get; set; }
        public DateTime Registration { get; set; } = DateTime.Now;
        public List<int> SelectedTrainingTypes { get; set; }
    }
}
