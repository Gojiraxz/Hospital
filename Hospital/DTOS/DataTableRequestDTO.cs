namespace Hospital.DTOS
{
    public class DataTableRequestDTO
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }

        public DataTableSeachDto? search { get; set; }
        public List<DataTableOrderDto> order { get; set; } = [];

        public List<DataTableColumnDto> columns { get; set; } = [];
    }
    public class DataTableSeachDto
    {
        public string? value { get; set; }
        public bool regex { get; set; }
    }
    public class DataTableOrderDto
    {
        public int column { get; set; }
        public string dir { get; set; } = string.Empty;
    }
    public class DataTableColumnDto
    {
        public string? data { get; set; } = string.Empty;
        public string? name { get; set; } = string.Empty;
        public bool searchable { get; set; }
        public bool orderable { get; set; }
        public DataTableSeachDto? search { get; set; }
    }
}
