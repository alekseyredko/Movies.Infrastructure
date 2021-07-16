using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FluentValidation;
using Movies.Data.Models;

namespace Movies.Infrastructure.Validators
{
    public class PersonValidator: AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleSet("PostPerson", () =>
            {
                RuleFor(x => x.PersonId).Empty();
                RuleFor(x => x.PersonName).NotEmpty();
            });

            RuleSet("PutPerson", () =>
            {
                RuleFor(x => x.PersonId).NotEmpty();
                RuleFor(x => x.PersonName).NotEmpty();
            });

            RuleSet("other", () =>
            {
                RuleFor(x => x.Actor).Null(); 
                RuleFor(x => x.Producer).Null();
                RuleFor(x => x.Reviewer).Null();
            });
        }
    }
}
