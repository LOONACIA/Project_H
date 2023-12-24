using System.Collections;

public interface IGate
{
    IEnumerator Open();

    IEnumerator Close();
}
