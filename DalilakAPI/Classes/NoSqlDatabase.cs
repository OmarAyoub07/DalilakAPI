using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents;
using System.Security.Cryptography.X509Certificates;
using Raven.Client.Documents.Session;
using DalilakAPI.Models;

namespace DalilakAPI.Classes
{
    public class NoSqlDatabase
    {
        /* Variables to Initialize Connection  */
        private X509Certificate2 clientCertificate; // Client Key
        private IDocumentSession session; // to access files (add , delete , update) - need to use this 'session'
        private string database; // database name
        private string[] serverUrls; // url for each Node within distributed System

        public NoSqlDatabase()
        {
            clientCertificate = new X509Certificate2(@"free.dalilak.client.certificate/free.dalilak.client.certificate.pfx",
                string.Empty,
                X509KeyStorageFlags.MachineKeySet); // RavenDB Verification 

            database = "Dalilak_DB";
            serverUrls = new[] { "https://a.free.dalilak.ravendb.cloud" }; // All Nodes of RavenDB - in our case we only have one Node

            establishSession(); // Initialize Connection
        }

        private void establishSession()
        {
            using IDocumentStore store = new DocumentStore()
            {
                Certificate = clientCertificate,
                Database = database,
                Urls = serverUrls

            }.Initialize(); // Connection properties stored at 'session'
            {
                session = store.OpenSession();
            }
        }


        /* Functions to add data */

        // - Add Comment to documnet
        public void AddComment(string docID, string userID, string message)
        {
            
            var doc = session.Load<Comments>(docID);

            if (!doc.reviewers.Any(usr => usr.user_id == userID))
                doc.reviewers.Add(new Reviewer { user_id = userID, like = false, reviews = new List<Review>() });

            var reviewer = doc.reviewers.Single(user => user.user_id == userID);
            DateTime datetime = DateTime.Now;
            reviewer.reviews.Add(new Review { comment = message, date = datetime.ToString("dd/MMM/yyyy"), time = datetime.ToString("hh:mm") });
            
            session.SaveChanges();
        }

        // Add Rate to Document
        public void AddRate(string docID, string placeID, int rate, bool favorit)
        {
            var doc = session.Load<History>(docID);

            if (!doc.records.Any(plc => plc.place_id == placeID))
                doc.records.Add(new Record { place_id = placeID, favorite = favorit, rate = rate});
            else
            {
                var record = doc.records.Single(plc => plc.place_id == placeID);
                record.rate = rate != 0 ? rate : record.rate;
                record.favorite = favorit != record.favorite ? favorit : record.favorite;
            }
            session.SaveChanges();
        }

        // Increase Statistics for place
        public void AddVisits(string docID, string date, string time, int visistsNum)
        {
            var doc = session.Load<Statistics>(docID);

            if (!doc.days.Any(day => day.date == date))
                doc.days.Add(new VisitDay { date = date, hours = new List<VisitTime>() });

            var day = doc.days.Single(day => day.date == date);

            if (!day.hours.Any(hour => hour.time == time))
                day.hours.Add(new VisitTime { time = time, visits_num = visistsNum });
            else
                day.hours.Single(hour => hour.time == time).visits_num += visistsNum ;

            session.SaveChanges();
        }
        
        // - Add Image to document
        public void AddImage(string docID, string img)
        {
            // Load the Json Document form RavenDB
            var doc = session.Load<Comments>(docID);

            doc.images.Add(img);

            session.SaveChanges();

        }
        /* End */


        /* Functions to Get Data */

        // - Get LinearRegression DataSet
        public List<DataSet.LinearRegression> Get_LR_DataSet()
        {
            var dataset = session.Query<DataSet.LinearRegression>("LinearRegression_DataSet").ToList();

            return dataset;
        }

        // - Get MatrixFactorization DataSet
        public List<DataSet.MatrixFactorization> Get_MF_DataSet()
        {
            var dataset = session.Query<DataSet.MatrixFactorization>("MatrixFactorization_DataSet").ToList();

            return dataset;
        }

        // - Get all comments
        public List<Reviewer> GetComments(string docID)
        {
            var doc = session.Load<Comments>(docID);

            return doc.reviewers;
        }

        // - Get One random Image
        public string GetImage_Random(string docID)
        {
            var doc = session.Load<Comments>(docID);
            Random rn = new Random();
            return doc.images[rn.Next(0, doc.images.Count())];
        }

        // - Get all Images
        public List<string> GetImages(string docID)
        {
            var doc = session.Load<Comments>(docID);
            return doc.images;
        }

        // - Get All Schedules for one user
        public List<Schedules> GetUserSchedules(List<string> DocsId)
        {
            List<Schedules> schedules = new List<Schedules>();
            foreach (var doc in DocsId)
            {
                schedules.Add(session.Load<Schedules>(doc));
            }
            return schedules;
        }

        // - Get History record
        public History GetHistory(string docId)
        {
            var doc = session.Load<History>(docId);
            return doc;
        }
        /* End */


        /* Creat Documnets */
        public string[] createNewDoc_forPlace(string placeId)
        {
            string[] doc = new string[2];
            doc[0] = Guid.NewGuid().ToString("D");
            doc[1] = Guid.NewGuid().ToString("D");

            session.Store(new Statistics
            {
                Id = doc[0],
                place_id = placeId,
                days = new List<VisitDay>()
            });
            session.Store(new Comments
            {
                Id = doc[1],
                place_id = placeId,
                images = new List<string>(),
                reviewers = new List<Reviewer>()
            });

            session.SaveChanges();
            return doc;
        }

        public string createNewDocforUser(string userId)
        {
            string doc = Guid.NewGuid().ToString("D");

            session.Store(new History { user_id = userId, Id = doc, records = new List<Record>() });
            session.SaveChanges();
            return doc;
           
        }

        public bool AddNewDocforSchdl(Schedules schedule)
        {
            try
            {
                session.Store(schedule);
                session.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /* End */
  

        /* Delete Function */
        public void deleteDoc(string id)
        {
            session.Delete(id);
            session.SaveChanges();
        }

        /* End */

    }
}
