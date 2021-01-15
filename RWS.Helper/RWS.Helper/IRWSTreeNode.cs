namespace RWS.Helper
{
    public interface IRWSTreeNode<T> {

        T Parent { get; set; }
        T[] Children { get; set; }
        T Data { get; }  
    }
}
