using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            switch (crudOperation)
            {

                case "create":
                    db.Employees.InsertOnSubmit(employee);
                    break;
                case "read":
                    var Worker = db.Employees.Where(e => e == employee).Select(e => e).Single();
                    Console.WriteLine("First Name: " + Worker.FirstName + "\n"
                                      + "Last Name: " + Worker.LastName + "\n"
                                      + "Employee Number: " + Worker.EmployeeNumber + "\n"
                                      + "Email Address: " + Worker.Email + "\n");
                    Console.ReadLine();
                    break;
                //case "update":

                case "delete":
                    db.Employees.DeleteOnSubmit(employee);
                    break;
                default:
                    break;
            }
            db.SubmitChanges();
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges(); // because it needs to update the db
        }

        internal static Animal GetAnimalByID(int id)
        {
            return db.Animals.Where(a => a.AnimalId == id).Select(a => a).FirstOrDefault();
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {            
            //
        }

        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            throw new NotImplementedException();
        }
         
        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            return db.Categories.Where(c => c.Name == categoryName).Select(c => c.CategoryId).Single();
        }
        
        internal static Room GetRoom(int animalId)
        {
            return db.Rooms.Where(r => r.AnimalId == animalId).Select(r => r).Single();
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            return db.DietPlans.Where(d => d.Name == dietPlanName).Select(d => d.DietPlanId).Single();
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            Adoption adoption = null; // the initiation of the process
            adoption.AnimalId = animal.AnimalId; // checking for a match
            adoption.ClientId = client.ClientId; //both match done
            adoption.ApprovalStatus = "Pending";

            adoption.AdoptionFee = 75; // mentioned in the customer class, apply for adoption method

            adoption.PaymentCollected = false;
            
            db.Adoptions.InsertOnSubmit(adoption);
            db.SubmitChanges();

           // db.Animals.Where(a => a.AnimalId == (db.Adoptions.Select(ad => ad.AnimalId) && db.Adoptions.Where(ad => ad.ClientId == (db.Clients.Select(cl => cl.ClientId)).Single();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            return db.Adoptions.Where(ad => ad.ApprovalStatus == "Pending").Select(ad => ad);
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            var newAdoption = db.Adoptions.Where(ad => ad.AnimalId == adoption.AnimalId && ad.ClientId == adoption.ClientId).Select(ad => ad).First();  

            if (isAdopted == true)
            {
                adoption.ApprovalStatus = "Approved";
                adoption.PaymentCollected = true;
            }
            else
            {
                adoption.ApprovalStatus = "Pending";
            }
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            throw new NotImplementedException();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
           return  db.AnimalShots.Where(s => s.AnimalId == animal.AnimalId).Select(a => a.ShotId).Single();
            //testing 
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            throw new NotImplementedException();
        }
    }
}