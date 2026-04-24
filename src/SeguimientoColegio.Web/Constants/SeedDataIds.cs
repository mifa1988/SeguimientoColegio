namespace SeguimientoColegio.Web.Constants;

public static class SeedDataIds
{
    public static class Roles
    {
        public const int Administrador = 1;
        public const int Editor = 2;
        public const int Consulta = 3;
    }

    public static class Estados
    {
        public const int Pendiente = 1;
        public const int EnProceso = 2;
        public const int Concluido = 3;
        public const int Descartado = 4;
        public const int Vencido = 5;
    }

    public static class Prioridades
    {
        public const int Baja = 1;
        public const int Media = 2;
        public const int Alta = 3;
        public const int Critica = 4;
    }

    public static class TiposRegistro
    {
        public const int MaterialRefuerzo = 1;
        public const int Requerimiento = 2;
        public const int Lectura = 3;
        public const int Tarea = 4;
        public const int AvisoImportante = 5;
        public const int AcuerdoProfesora = 6;
        public const int Actividad = 7;
        public const int Evaluacion = 8;
    }

    public static class Plataformas
    {
        public const int Wordwall = 1;
        public const int Odilo = 2;
        public const int Canva = 3;
        public const int Pdf = 4;
        public const int Youtube = 5;
        public const int GoogleDrive = 6;
        public const int Otro = 7;
    }

    public static class Disciplinas
    {
        public const int Matematicas = 1;
        public const int Danza = 2;
        public const int Lectoescritura = 3;
        public const int Comunicacion = 4;
        public const int PlanLector = 5;
        public const int Arte = 6;
    }

    public static class Ninos
    {
        public const int Daniela = 1;
    }
}
