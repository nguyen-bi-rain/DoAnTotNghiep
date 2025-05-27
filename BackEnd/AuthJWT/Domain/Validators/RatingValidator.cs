// using AuthJWT.Domain.Entities.Common;
// using FluentValidation;

// namespace AuthJWT.Domain.Validators
// {
//     public class RatingValidator : AbstractValidator<Rating>
//     {
//         public RatingValidator()
//         {
//             RuleFor(p => p.RatingValue).NotEmpty()
//                 .WithMessage("Rating value is required.")
//                 .InclusiveBetween(1, 5)
//                 .WithMessage("Rating value must be between 1 and 5.");
//             RuleFor(p => p.Title).NotEmpty().WithMessage("title must not empty");

//         }
//     }
// }
