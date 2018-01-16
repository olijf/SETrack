﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCF_Demo.Contracts;
using WCF_Demo.Data;

namespace WCF_Demo.Services
{
    public class CheckAstronautService : ICheckAstronautsService
    {
        private const int DaysPerYear = 365; //from configuration
        private const int MaximumAge = 60; //from configuration

        public bool DoFinalCheckup(FlightRosterData data)
        {
            var astronauts = GetAstronautsFromService(data);
            foreach (var astronaut in astronauts)
            {
                if (astronaut.Clearance == NASAClearance.Pending || astronaut.Clearance == NASAClearance.Limited)
                {
                    return false;
                }
                if (astronaut.LastMedicalCheck < new DateTime(2015, 1, 1))
                {
                    return false;
                }
                if (astronaut.Birthdate < DateTime.Now.Subtract(TimeSpan.FromDays(DaysPerYear * MaximumAge)))
                {
                    return false;
                }
            }
            return true;
        }

        public bool DoPreliminaryCheckup(FlightRosterData data)
        {
            var astronauts = GetAstronautsFromService(data);
            foreach (var astronaut in astronauts)
            {
                if (astronaut.LastMedicalCheck < new DateTime(2010, 1, 1))
                {
                    return false;
                }
                if (astronaut.Birthdate < DateTime.Now.Subtract(TimeSpan.FromDays(DaysPerYear * MaximumAge)))
                {
                    return false;
                }
            }
            return true;
        }

        public AstronautData GetAstronaut(string name)
        {
            var astronaut = GetAstronautByName(name);
            var result = new AstronautData
            {
                Name = name,
                Id = astronaut.ID,
                Nationality = astronaut.Nationality,
                FullyCleared = astronaut.Clearance == NASAClearance.Full
            };
            return result;
        }

        private Astronaut GetAstronautByName(string name)
        {
            var db = new AstronautsDB(); //should be done via dependency injection
            return db.GetAstronaut(name);
        }

        private List<Astronaut> GetAstronautsFromService(FlightRosterData data)
        {
            var db = new AstronautsDB(); //should be done via dependency injection
            var result = new List<Astronaut>
            {
                db.GetAstronaut(data.CaptainName),
                db.GetAstronaut(data.SecondInCommandName),
                db.GetAstronaut(data.NavigatorName),
                db.GetAstronaut(data.EngineerName)
            };
            return result;
        }
    }
}