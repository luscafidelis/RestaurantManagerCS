using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManager {
    public static class Message {
        public static string DatabaseError() {
            return "Ocorreu um erro ao se comunicar com o banco de dados. \nComunique o administrador.";
        }
    }
}
