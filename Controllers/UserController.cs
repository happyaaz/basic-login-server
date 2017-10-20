using BasicLoginServer.CustomLibraries;
using BasicLoginServer.Helpers;
using BasicLoginServer.Managers;
using BasicLoginServer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BasicLoginServer.Controllers
{
    [Authorize(Roles = "Admin")]
    [RequireSSL]
    public class UserController : Controller
    {


        void SetTypeOfActionWithRequestForm(string typeOfActionWithRequestForm)
        {
            ViewBag.TypeOfActionWithRequestForm = typeOfActionWithRequestForm;
        }

        void SetRolesForViewBag()
        {
            ViewBag.DefinedRoles = CommonManager.ReturnAvailableRoles();
        }
        // GET: User
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                //List<UserInfo> usersInfo = GetListOfUsers();
                return View();
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);
                return HttpNotFound("Something went wrong. Please, contact the administration");
            }
        }


        private List<UserInfo> GetListOfUsers()
        {
            string connectionString = CommonManager.ReturnNeededConnectionStringForHotel();
            // select only those people who exist in the DB
            string sqlToGetUsersInfo = @"SELECT userId, userType, userFullName, userUniqueDatabaseId, userEmail, userPhoneNumber
                    FROM tblUserInformation WHERE
                    NOT userId=@userId AND ifRemoved='False'";
            List<UserInfo> usersInfo = new List<UserInfo>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmdToGetUsersInfo = new SqlCommand(sqlToGetUsersInfo, conn);
                cmdToGetUsersInfo.Parameters.AddWithValue("@userId", GetCurrentClaimValues.GetCurrentUserName());



                using (SqlDataReader dt_ToGetUsersInfo = cmdToGetUsersInfo.ExecuteReader())
                {
                    if (dt_ToGetUsersInfo.HasRows)
                    {
                        while (dt_ToGetUsersInfo.Read())
                        {
                            UserInfo ui = new UserInfo
                            {
                                userId = dt_ToGetUsersInfo[0].ToString().TrimEnd(),
                                userType = dt_ToGetUsersInfo[1].ToString().TrimEnd(),
                                userFullName = dt_ToGetUsersInfo[2].ToString().TrimEnd(),
                                uniqueUserId = dt_ToGetUsersInfo[3].ToString().TrimEnd(),
                                userEmail = dt_ToGetUsersInfo[4].ToString().TrimEnd(),
                                userPhoneNumber = dt_ToGetUsersInfo[5].ToString().TrimEnd()
                            };
                            usersInfo.Add(ui);
                        }
                    }
                }
            }
            return usersInfo;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetJsonedListOfUsers(string token)
        {
            try
            {
                string userAgent = Request.Headers["User-Agent"].ToString();
                bool result = CommonManager.IsTokenValid(token, CommonManager.GetIP(Request), userAgent);

                if (result == false)
                {
                    return Json(false);
                }
                else
                {
                    List<UserInfo> usersInfo = GetListOfUsers();
                    return Json(JsonConvert.SerializeObject(usersInfo));
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);
                return HttpNotFound("Something went wrong. Please, contact the administration");
            }
        }


        [HttpGet]
        public ActionResult EditUser(string uniqueUserId)
        {
            if (uniqueUserId == "" || uniqueUserId == null)
            {
                return HttpNotFound("Something went wrong. Please, contact the administration");
            }
            try
            {
                SetRolesForViewBag();
                SetTypeOfActionWithRequestForm("Edit");

                string connectionStringCommon = CommonManager.ReturnNeededConnectionStringForCommonDatabase();

                // select only those people who exist in the DB
                string sqlToGetUsersInfo = @"SELECT userId, userUniqueDatabaseId, userPwd FROM tblUser 
                            WHERE userUniqueDatabaseId = @userUniqueDatabaseId";

                UserInfo ui = new UserInfo();

                using (SqlConnection conn = new SqlConnection(connectionStringCommon))
                {
                    conn.Open();
                    SqlCommand cmdToGetUsersInfo = new SqlCommand(sqlToGetUsersInfo, conn);
                    cmdToGetUsersInfo.Parameters.AddWithValue("@userUniqueDatabaseId", uniqueUserId);


                    using (SqlDataReader dt_ToGetUsersInfo = cmdToGetUsersInfo.ExecuteReader())
                    {
                        if (dt_ToGetUsersInfo.HasRows)
                        {
                            while (dt_ToGetUsersInfo.Read())
                            {
                                ui = new UserInfo
                                {
                                    userId = dt_ToGetUsersInfo[0].ToString().TrimEnd(),
                                    uniqueUserId = dt_ToGetUsersInfo[1].ToString().TrimEnd(),
                                    userPwd = CustomDecrypt.Decrypt(dt_ToGetUsersInfo[2].ToString().TrimEnd())
                                };
                            }
                        }
                    }
                }

                string connectionStringHotel = CommonManager.ReturnNeededConnectionStringForHotel();

                // select only those people who exist in the DB
                string sqlToGetMainUsersInfo = @"SELECT userType, userFullName, userEmail, userPhoneNumber FROM tblUserInformation 
                            WHERE userUniqueDatabaseId = @uniqueUserId";

                using (SqlConnection conn = new SqlConnection(connectionStringHotel))
                {
                    conn.Open();
                    SqlCommand cmdToGetUsersInfo = new SqlCommand(sqlToGetMainUsersInfo, conn);
                    cmdToGetUsersInfo.Parameters.AddWithValue("@uniqueUserId", ui.uniqueUserId);


                    using (SqlDataReader dt_ToGetUsersInfo = cmdToGetUsersInfo.ExecuteReader())
                    {
                        if (dt_ToGetUsersInfo.HasRows)
                        {
                            while (dt_ToGetUsersInfo.Read())
                            {
                                ui.userType = dt_ToGetUsersInfo[0].ToString().TrimEnd();
                                ui.userFullName = dt_ToGetUsersInfo[1].ToString().TrimEnd();
                                ui.userEmail = dt_ToGetUsersInfo[2].ToString().TrimEnd();
                                ui.userPhoneNumber = dt_ToGetUsersInfo[3].ToString().TrimEnd();
                            }
                        }
                    }
                }
                return View(ui);
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);
                return HttpNotFound("Something went wrong. Please, contact the administration");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult EditUser(UserInfo ui)
        {
            SetRolesForViewBag();
            SetTypeOfActionWithRequestForm("Edit");
            if (!ModelState.IsValid) //Checks if input fields have the correct format
            {
                return View(ui); //Returns the view with the input values so that the user doesn't have to retype again
            }
            try
            {
                if (CheckIfSuchUserAlreadyExistsInDatabase(ui.userId, true) == true)
                {
                    ModelState.AddModelError("userId", "Username must be unique");
                    return View(ui);
                }
                else
                {
                    string connectionStringCommon = CommonManager.ReturnNeededConnectionStringForCommonDatabase();
                    string decryptedPassword = CustomEncrypt.Encrypt(ui.userPwd);
                    string sqlToUpdateInfoAboutUser = @"UPDATE tblUser SET 
                                userId=@userId,
                                userPwd=@userPwd
                                WHERE userUniqueDatabaseId = @userUniqueDatabaseId";

                    using (SqlConnection conn = new SqlConnection(connectionStringCommon))
                    {
                        conn.Open();
                        SqlCommand cmdToUpdateInfoAboutUser = new SqlCommand(sqlToUpdateInfoAboutUser, conn);

                        cmdToUpdateInfoAboutUser.Parameters.AddWithValue("@userId", ui.userId);
                        cmdToUpdateInfoAboutUser.Parameters.AddWithValue("@userPwd", decryptedPassword);
                        cmdToUpdateInfoAboutUser.Parameters.AddWithValue("@userUniqueDatabaseId", ui.uniqueUserId);

                        cmdToUpdateInfoAboutUser.ExecuteNonQuery();
                    }


                    string connectionStringHotel = CommonManager.ReturnNeededConnectionStringForHotel();

                    string sqlToUpdateMainInfoAboutUser = @"UPDATE tblUserInformation  SET 
                                userType=@userType,
                                userFullName=@userFullName,
                                userEmail=@userEmail,
                                userId=@userId,
                                userPhoneNumber=@userPhoneNumber
                                WHERE userUniqueDatabaseId = @userUniqueDatabaseId";

                    using (SqlConnection conn = new SqlConnection(connectionStringHotel))
                    {
                        conn.Open();
                        SqlCommand cmdToUpdateInfoAboutUser = new SqlCommand(sqlToUpdateMainInfoAboutUser, conn);

                        cmdToUpdateInfoAboutUser.Parameters.AddWithValue("@userType", ui.userType);
                        cmdToUpdateInfoAboutUser.Parameters.AddWithValue("@userFullName", ui.userFullName);
                        cmdToUpdateInfoAboutUser.Parameters.AddWithValue("@userUniqueDatabaseId", ui.uniqueUserId);
                        cmdToUpdateInfoAboutUser.Parameters.AddWithValue("@userEmail", ui.userEmail);
                        cmdToUpdateInfoAboutUser.Parameters.AddWithValue("@userId", ui.userId);
                        cmdToUpdateInfoAboutUser.Parameters.AddWithValue("@userPhoneNumber", ui.userPhoneNumber);

                        cmdToUpdateInfoAboutUser.ExecuteNonQuery();
                    }


                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);
                return HttpNotFound("Something went wrong. Please, contact the administration");
            }
        }


        [HttpGet]
        public ActionResult DeleteUser(string uniqueUserId)
        {
            if (uniqueUserId == "" || uniqueUserId == null)
            {
                return HttpNotFound("Something went wrong. Please, contact the administration");
            }
            try
            {
                string connectionStringCommon = CommonManager.ReturnNeededConnectionStringForCommonDatabase();

                string sqlToDeleteUserCommon = @"UPDATE tblUser SET ifRemoved=@ifRemoved WHERE userUniqueDatabaseId = @userUniqueDatabaseId";
                using (SqlConnection conn = new SqlConnection(connectionStringCommon))
                {
                    conn.Open();
                    SqlCommand cmdToDeleteUser = new SqlCommand(sqlToDeleteUserCommon, conn);
                    cmdToDeleteUser.Parameters.AddWithValue("@userUniqueDatabaseId", uniqueUserId);
                    cmdToDeleteUser.Parameters.AddWithValue("@ifRemoved", 1);

                    cmdToDeleteUser.ExecuteNonQuery();
                }


                string connectionString = CommonManager.ReturnNeededConnectionStringForHotel();

                string sqlToDeleteUserHotel = @"UPDATE tblUserInformation SET ifRemoved=@ifRemoved WHERE userUniqueDatabaseId = @userUniqueDatabaseId";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmdToDeleteUser = new SqlCommand(sqlToDeleteUserHotel, conn);
                    cmdToDeleteUser.Parameters.AddWithValue("@userUniqueDatabaseId", uniqueUserId);
                    cmdToDeleteUser.Parameters.AddWithValue("@ifRemoved", 1);

                    cmdToDeleteUser.ExecuteNonQuery();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);
                return HttpNotFound("Something went wrong. Please, contact the administration");
            }
        }


        [HttpGet]
        public ActionResult CreateUser()
        {
            SetRolesForViewBag();
            SetTypeOfActionWithRequestForm("Create");
            return View(new UserInfo());
        }


        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult CreateUser([Bind(Exclude = "uniqueUserId")]UserInfo ui)
        {
            SetRolesForViewBag();
            SetTypeOfActionWithRequestForm("Create");
            if (!ModelState.IsValid) //Checks if input fields have the correct format
            {
                return View(ui); //Returns the view with the input values so that the user doesn't have to retype again
            }
            else
            {
                try
                {

                    string decryptedPassword = CustomEncrypt.Encrypt(ui.userPwd);

                    if (CheckIfSuchUserAlreadyExistsInDatabase(ui.userId, false) == true)
                    {
                        ModelState.AddModelError("userId", "Username must be unique");
                        return View(ui);
                    }
                    else
                    {
                        string uniqueEightDigitNumber = GenerateUniqueValues.ReturnUniqueEightDigitNumber();

                        string connectionStringCommon = CommonManager.ReturnNeededConnectionStringForCommonDatabase();
                        string sqlToCreateUser = @"INSERT INTO tblUser (userId, userPwd, userEmployer, userUniqueDatabaseId, ifRemoved) VALUES
                                (@userId, @userPwd, @userEmployer, @userUniqueDatabaseId, @ifRemoved)";

                        using (SqlConnection conn = new SqlConnection(connectionStringCommon))
                        {
                            conn.Open();

                            SqlCommand cmdToCreateUser = new SqlCommand(sqlToCreateUser, conn);

                            cmdToCreateUser.Parameters.AddWithValue("@userId", ui.userId);
                            cmdToCreateUser.Parameters.AddWithValue("@userPwd", decryptedPassword);
                            cmdToCreateUser.Parameters.AddWithValue("@userEmployer", GetCurrentClaimValues.GetCurrentUserEmployer());
                            cmdToCreateUser.Parameters.AddWithValue("@userUniqueDatabaseId", uniqueEightDigitNumber);
                            cmdToCreateUser.Parameters.AddWithValue("@ifRemoved", 0);

                            cmdToCreateUser.ExecuteNonQuery();
                        }


                        string connectionStringHotel = CommonManager.ReturnNeededConnectionStringForHotel();
                        string sqlToCreateInfoAboutUser = @"INSERT INTO tblUserInformation (userFullName, userType,
                                userId, userUniqueDatabaseId, userEmail, userPhoneNumber, ifRemoved) VALUES
                                (@userFullName, @userType, @userId, @userUniqueDatabaseId, @userEmail, @userPhoneNumber, @ifRemoved)";

                        using (SqlConnection conn = new SqlConnection(connectionStringHotel))
                        {
                            conn.Open();
                            SqlCommand cmdToCreateMainUser = new SqlCommand(sqlToCreateInfoAboutUser, conn);

                            cmdToCreateMainUser.Parameters.AddWithValue("@userId", ui.userId);
                            cmdToCreateMainUser.Parameters.AddWithValue("@userFullName", ui.userFullName);
                            cmdToCreateMainUser.Parameters.AddWithValue("@userType", ui.userType);
                            cmdToCreateMainUser.Parameters.AddWithValue("@userUniqueDatabaseId", uniqueEightDigitNumber);
                            cmdToCreateMainUser.Parameters.AddWithValue("@userEmail", ui.userEmail);
                            cmdToCreateMainUser.Parameters.AddWithValue("@userPhoneNumber", ui.userPhoneNumber);
                            cmdToCreateMainUser.Parameters.AddWithValue("@ifRemoved", 0);

                            cmdToCreateMainUser.ExecuteNonQuery();

                            if (ui.userType == "Driver")
                            {
                                string sqlDriverAvailability = @"INSERT INTO tblDriverAvailability VALUES 
                                        (@driverUniqueId, @driverAvailability)";
                                SqlCommand cmdToCreateDriverAvailability = new SqlCommand(sqlDriverAvailability, conn);

                                cmdToCreateDriverAvailability.Parameters.AddWithValue("@driverUniqueId", uniqueEightDigitNumber);
                                cmdToCreateDriverAvailability.Parameters.AddWithValue("@driverAvailability", true);
                                cmdToCreateDriverAvailability.ExecuteNonQuery();
                            }
                        }


                        return RedirectToAction("Index");
                    }

                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex);
                    return HttpNotFound("Something went wrong. Please, contact the administration");
                }
            }
        }


        bool CheckIfSuchUserAlreadyExistsInDatabase(string userId, bool edit)
        {
            string connectionString = CommonManager.ReturnNeededConnectionStringForCommonDatabase();
            string sqlToCheckIfUserAlreadyExists = string.Empty;
            if (edit == false)
            {
                sqlToCheckIfUserAlreadyExists = @"SELECT TOP 1 id FROM tblUser 
                WHERE userId = @userId";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmdToCheckIfUserAlreadyExists = new SqlCommand(sqlToCheckIfUserAlreadyExists, conn);
                    cmdToCheckIfUserAlreadyExists.Parameters.AddWithValue("@userId", userId);

                    SqlDataReader dt_toCheckIfUserAlreadyExists = cmdToCheckIfUserAlreadyExists.ExecuteReader();

                    if (dt_toCheckIfUserAlreadyExists.HasRows)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {

                sqlToCheckIfUserAlreadyExists = @"SELECT COUNT (*) FROM tblUser WHERE userId = @userId";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmdToCheckIfUserAlreadyExists = new SqlCommand(sqlToCheckIfUserAlreadyExists, conn);
                    cmdToCheckIfUserAlreadyExists.Parameters.AddWithValue("@userId", userId);

                    int numberOfPeopleWithSameId = (int)cmdToCheckIfUserAlreadyExists.ExecuteScalar();

                    if (numberOfPeopleWithSameId > 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
}