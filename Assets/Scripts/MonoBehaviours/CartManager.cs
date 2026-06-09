using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Schema;
using UnityEngine;

public class CartManager : MonoBehaviour
{
    public static CartManager instance { get; set; }
    [SerializeField] private List<string> cart = new();
    [SerializeField] private List<string> pastCarts = new();
    [SerializeField] private List<string> pastRequired = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public List<string> GetCartList()
    {
        return cart; 
    }

    public void AddToCart(string text)
    {
        cart.Add(text);
    }

    public void RemoveFromCart(string text)
    {
        cart.Remove(text);
    }

    public void ClearCart()
    {
        cart.Clear();
    }

    public int GetCount()
    {
        return cart.Count;
    }

    public void SaveCartHistory()
    {
        foreach (var item in cart)
        {
            pastCarts.Add(item);
        }
    }

    public void SaveRequiredHistory(List<string> list)
    {
        foreach (var item in list)
        {
            pastRequired.Add(item);
        }
    }

    public void PrintCartContents()
    {
        string output = "All cart items: ";
        foreach (var item in cart)
        {
            output += item + " ";
        }

        Debug.Log(output);
    }
}
