using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Ultils
{
    public static class Custom
    {
        public static IRuleBuilderOptions<T, TProperty> ValidationFile<T,TProperty>(this IRuleBuilder<T,TProperty> builder, IEnumerable<string> AllowExtention, int FileSize)
        {
            return builder.SetValidator(new FileValidation<T,TProperty>(AllowExtention, FileSize));
        }
    }

    public class FileValidation<T, TProperty> : PropertyValidator<T, TProperty>
    {
        private IEnumerable<string> AllowExtention {  get; set; }
        private int size {  get; set; }
        public FileValidation(IEnumerable<string> allowExtention, int size)
        {
            AllowExtention = allowExtention;
            this.size = size;
        }

        public override string Name => "FileValidation";

        public override bool IsValid(ValidationContext<T> context, TProperty value)
        {
            
           

            if (typeof(TProperty).Equals(typeof(IFormFile)) && value is not null){

                var file = (IFormFile) value!;

                var fileExtention = Path.GetExtension(file.FileName);

                var fileSize = (file.Length / 1024);

                if (fileSize > size || size<=1 )
                {
                   
                    return false;
                }

                foreach (var item in AllowExtention)
                {
                    if (item.Equals(fileExtention))
                    {
                        return true;
                    }
                }


               
                return false;

            }
            else
            {
                throw new Exception("Only using validation IFromFile"!);
            }

            

            
           
        }

        protected override string GetDefaultMessageTemplate(string errorCode)
        {
            
            return "File not valid!";
        }


    }
}
