namespace Hospital.DTOS
{
    public class UserPermissionOverride
    {
      public int permissionId { get; set; }
        public string permissionName { get; set; } = string.Empty;
        public bool IsGranted { get; set; }
    }
}
