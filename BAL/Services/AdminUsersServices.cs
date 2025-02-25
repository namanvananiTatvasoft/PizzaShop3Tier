using BAL.Interfaces;
using DAL.Database;
using DAL.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace BAL.Services;

public class AdminUsersServices : IAdminUsersServices
{
    private readonly PizzaShopDbContext _db;
    private readonly IEmailServices _emailServices;

    public AdminUsersServices(PizzaShopDbContext db, IEmailServices emailServices)
    {
        _db = db;
        _emailServices = emailServices;
    }
    public async Task<(List<UserListModel>, int Count, int PageSize, int PageNumber, string SortColumn, string SortDirection, string SearchKey)> getDynamicUserList(int PageSize, int PageNumber, string SortColumn, string SortDirection, string SearchKey)
    {
        var query = from user in _db.Userdetails
                    join role in _db.Roles on user.Roleid equals role.Roleid
                    where user.Isdeleted == false &&
                        (user.Firstname.ToLower().Contains(SearchKey) ||
                         user.Lastname.ToLower().Contains(SearchKey) ||
                         user.Email.ToLower().Contains(SearchKey) ||
                         role.Rolename.ToLower().Contains(SearchKey) ||
                         user.Phone.ToLower().Contains(SearchKey))
                    select new UserListModel
                    {
                        Email = user.Email,
                        Phone = user.Phone,
                        Firstname = user.Firstname,
                        Lastname = user.Lastname,
                        Role = role.Rolename,
                        Status = user.Status
                    };

        // Sorting logic based on the column and direction
        if (SortColumn == "Firstname")
        {
            query = SortDirection == "asc" ? query.OrderBy(u => u.Firstname) : query.OrderByDescending(u => u.Firstname);
        }
        else if (SortColumn == "Role")
        {
            query = SortDirection == "asc" ? query.OrderBy(u => u.Role) : query.OrderByDescending(u => u.Role);
        }

        var count = await query.CountAsync();

        // Ensure the PageNumber is at least 1 (no less than 1)
        if (PageNumber < 1)
        {
            PageNumber = 1;
        }

        // Calculate the total number of pages (round up to the nearest whole number)
        var totalPages = (int)Math.Ceiling((double)count / PageSize);

        // Ensure the PageNumber does not exceed the total number of pages
        if (PageNumber > totalPages)
        {
            PageNumber = totalPages;
        }
        if (PageNumber < 1)
        {
            PageNumber = 1;
        }

        var userList = await query.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToListAsync();

        return (userList, count, PageSize, PageNumber, SortColumn, SortDirection, SearchKey);
    }


    public async Task SendEmailToNewUser(string email, string subject, string username, string password)
    {
        var message = @"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {
                        font-family: Arial, sans-serif;
                        color: #333;
                        margin: 0;
                        padding: 20px;
                        background-color: #f7f7f7;
                    }
                    .container {
                        background-color: #ffffff;
                        border-radius: 8px;
                        padding: 20px;
                        max-width: 600px;
                        margin: 0 auto;
                        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                    }
                    .header {
                        text-align: center;
                        color: #4CAF50;
                        font-size: 24px;
                        font-weight: bold;
                        margin-bottom: 20px;
                    }
                    .message {
                        font-size: 16px;
                        line-height: 1.6;
                    }
                    .highlight {
                        color: #4CAF50;
                        font-weight: bold;
                    }
                    .footer {
                        font-size: 12px;
                        color: #777;
                        margin-top: 20px;
                        text-align: center;
                    }
                    .footer a {
                        color: #4CAF50;
                        text-decoration: none;
                    }
                </style>
            </head>
            <body>

                <div class='container'>
                    <div class='header'>
                        Welcome to Pizza Shop!
                    </div>

                    <div class='message'>
                        <p>Hello <span class='highlight'>" + username + @"</span>,</p>
                        <p>Your account has been created successfully.</p>
                        <p><span class='highlight'>Username:</span>" + username + @"</p>
                        <p><span class='highlight'>Password:</span>" + password + @"</p>
                        <p>Please login to your account and <strong>change your password</strong> for security purposes.</p>
                    </div>

                    <div class='footer'>
                        <p>If you have any questions, feel free to <a href='mailto:support@pizzashop.com'>contact us</a>.</p>
                    </div>
                </div>

            </body>
            </html>
        ";
        await _emailServices.SendEmailAsync(email, subject, message);
    }
}
