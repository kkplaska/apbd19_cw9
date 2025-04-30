using System.ComponentModel.DataAnnotations;

namespace apbd19_cw9.Model;

public class Animal
{
    
    public int IdAnimal { get; set; }
    [MaxLength(200)]
    public string Name { get; set; }
}