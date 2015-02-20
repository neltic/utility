// tip      - El namespace se puede omitir para reducir el texto de transporte en JSON
// original - namespace neltic.model
using System;

public class Layer
{
    public int Id { get; set; }
    public string Model { get; set; }
    public double Cost { get; set; }
    public DateTime Added { get; set; }
    public bool Active { get; set; }
}
