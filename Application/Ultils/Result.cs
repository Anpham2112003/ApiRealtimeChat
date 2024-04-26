using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Ultils
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public Error Error { get; }
        public bool IsFailuer => !IsSuccess;
        public T? Data { get; }
        private Result(bool IsSuccess, Error error)
        {

            if(IsSuccess&& error!=Error.None||!IsSuccess&&error==Error.None) {
                throw new ArgumentException("Result Error not Valid");
            }

            this.IsSuccess = IsSuccess;
            this.Error = error;
        }

        private Result(bool IsSuccess, Error error,T Data)
        {

            if (IsSuccess && error != Error.None || !IsSuccess && error == Error.None)
            {
                throw new ArgumentException("Result Error not Valid");
            }

            this.IsSuccess = IsSuccess;
            this.Error = error;
            this.Data = Data;
        }

        public static Result<T> Success()=>new Result<T>(true, Error.None);
        public static Result<T> Failuer(Error error) => new Result<T>(false, error);
        public static Result<T> Success(T? Data)=>  new Result<T>(true,Error.None,Data);
    }
}
