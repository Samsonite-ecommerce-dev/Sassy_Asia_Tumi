namespace Samsonite.Library.Basic.Models
{
    public class EditPasswordRequest
    {
        public int UserID { get; set; }

        public string OldPassword { get; set; }

        public string Password { get; set; }

        public string SurePassword { get; set; }
    }
}
