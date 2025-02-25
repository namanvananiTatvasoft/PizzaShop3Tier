using DAL.ViewModel;

namespace BAL.Interfaces;

public interface IAdminUsersServices
{
    public  Task<(List<UserListModel>, int Count, int PageSize, int PageNumber, string SortColumn, string SortDirection, string SearchKey)> getDynamicUserList(int PageSize, int PageNumber, string SortColumn, string SortDirection, string SearchKey);

    public Task SendEmailToNewUser(string email, string subject, string username, string password);
}
