namespace UniTrainingSystem.Models
{
    public class CompanyTraining
    {
        public int CompanyTrainingId { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public int TrainingTypeId { get; set; }
        public TrainingType TrainingType { get; set; }
    }
}
