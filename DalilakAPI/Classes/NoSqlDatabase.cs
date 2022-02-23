﻿using System;
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
        private X509Certificate2 clientCertificate;
        private IDocumentSession session;
        private string database;
        private string[] serverUrls;

        public NoSqlDatabase()
        {
            clientCertificate = new X509Certificate2(@"free.dalilak.client.certificate/free.dalilak.client.certificate.pfx",
                string.Empty,
                X509KeyStorageFlags.MachineKeySet);

            database = "Dalilak_DB";
            serverUrls = new[] { "https://a.free.dalilak.ravendb.cloud" };

            establishSession();
        }

        private void establishSession()
        {
            using IDocumentStore store = new DocumentStore()
            {
                Certificate = clientCertificate,
                Database = database,
                Urls = serverUrls

            }.Initialize();
            {
                session = store.OpenSession();
            }
        }

        public void insertSchedule(string docId, string userId, string cityId, List<TripDay> daysLi)
        {
            session.Store(new Schedules
            {
                Id = docId,
                city_id = cityId,
                user_id = userId,
                days = daysLi
            });
            session.SaveChanges();
        }

        public void insertStatistics(string docId, string placeId, List<VisitDay> daysLi)
        {
            session.Store(new Statistics
            {
                place_id = placeId,
                Id = docId,
                days = daysLi
            });
            session.SaveChanges();
        }

        public void insertComment()
        {

        }

        public void insertHistory()
        {

        }

        public List<Statistics> selectStatistics()
        {
            using (session)
            {
                return session.Query<Statistics>().ToList();
            }
        }
        public string[] createNewDoc_forPlace(string placeId)
        {
            string[] doc = new string[2];
            doc[0] = Guid.NewGuid().ToString("D");
            doc[1] = Guid.NewGuid().ToString("D");

            session.Store(new Statistics
            {
                Id = doc[0],
                place_id = placeId
            });
            session.Store(new Comments
            {
                Id = doc[1],
                place_id = placeId
            });

            session.SaveChanges();
            return doc;
        }
        public void deleteDoc(string id)
        {
            session.Delete(id);
            session.SaveChanges();
        }

        public List<Comments> selectComments()
        {
            using (session)
            {
                return session.Query<Comments>().ToList();
            }
        }

        public List<History> selectHistories()
        {
            using (session)
            {
                return session.Query<History>().ToList();
            }
        }

        public List<Schedules> selectSchedules()
        {
            using (session)
            {
                return session.Query<Schedules>().ToList();
            }
        }

        //16cd4400-3398-4480-a096-1b29f7dee3c4

        public Statistics selectStatistics(string id)
        {
            using (session)
            {
                return session.Load<Statistics>(id);
            }

        }
    }
}
