namespace JT.Abp.Authorization.Users
{
    public class JTUserPermission : JTPermission
    {
        public virtual long UserId { get; set; }
    }
}
