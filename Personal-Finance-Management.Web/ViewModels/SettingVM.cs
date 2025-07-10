namespace Personal_Finance_Management.Web.ViewModels
{
    public class SettingVM
    {
        public UpdateUserDetailsVM userDetails { get; set; }
        public UpdatePasswordVM updatePassword { get; set; }
        public string ActiveTab { get; set; } = "Details";
    }
}
