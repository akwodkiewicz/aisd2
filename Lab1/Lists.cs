
using System;
using System.Collections.Generic;

namespace ASD
{

public interface IList : IEnumerable<int>
    {
    // Je�li element v jest na li�cie to zwraca true
    // Je�li elementu v nie ma na li�cie to zwraca false
    bool Search(int v);

    // Je�li element v jest na li�cie to zwraca false (elementu nie dodaje)
    // Je�li elementu v nie ma na li�cie to dodaje go do listy i zwraca true
    bool Add(int v);

    // Je�li element v jest na li�cie to usuwa go z listy i zwraca true
    // Je�li elementu v nie ma na li�cie to zwraca false
    bool Remove(int v);
    }

//
// dopisa� klas� opisuj�c� element listy
//
public class Lista
    {
        public int value;
        public Lista next = null;
        public Lista(int v, Lista n)
        {
            value = v;
            next = n;
        }
    }
// Zwyk�a lista (nie samoorganizuj�ca si�)
public class SimpleList : IList
    {

        // doda� niezb�dne pola
        public Lista head = null;
    // Lista si� nie zmienia
    public bool Search(int v)
        {
            Lista tmp = head;
            while (tmp != null)
            {
                if (tmp.value == v)
                    return true;
                tmp = tmp.next;
            }
            return false;
        }

    // Element jest dodawany na koniec listy
    public bool Add(int v)
        {
            Lista tmp = head;
            if (tmp == null)
            {
                head = new Lista(v, null);
                return true;
            }
            while (tmp.next != null)
            {
                if (tmp.value == v)
                    return false;
                tmp = tmp.next;
            }
            if (tmp.value == v) return false;
            else
            {
                tmp.next = new Lista(v, null);
                return true;
            }
        }

    // Pozosta�e elementy nie zmieniaj� kolejno�ci
    public bool Remove(int v)
        {
            if (head == null)
                return false;
            if(head.value == v)
            {
                head = head.next;
                return true;
            }
            Lista tmp = head;
            while (tmp.next != null)
            {
                if(tmp.next.value == v)
                {
                    Lista p = tmp.next;
                    tmp.next = p.next;
                    return true;
                }
                tmp = tmp.next;
            }

            return false;
        }

    // Wymagane przez interfejs IEnumerable<int>
    public IEnumerator<int> GetEnumerator()
        {
            // nie wolno modyfikowa� kolekcji
            Lista tmp = head;
            while(tmp!=null)
            {
                yield return tmp.value;
                tmp = tmp.next;
            }
       // zmieni�
        }

    // Wymagane przez interfejs IEnumerable<int> - nie zmmienia� (jest gotowe!)
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
        return this.GetEnumerator();
        }

    } // class SimpleList


// Lista z przesnoszeniem elementu, do kt�rego by� dost�p na pocz�tek
public class MoveToFrontList : IList
    {

        // doda� niezb�dne pola
        public Lista head = null;

    // Znaleziony element jest przenoszony na pocz�tek
        public bool Search(int v)
        {
            Lista ptmp = null;
            Lista tmp = head;
            while (tmp != null)
            {

                if (tmp.value == v)
                {
                    if (tmp != head)
                    {
                        if (ptmp != null)
                            ptmp.next = tmp.next;
                        tmp.next = head;
                        head = tmp;
                    }
                    return true;
                }
                    
                ptmp = tmp;
                tmp = tmp.next;
            }
            return false;
        }

    // Element jest dodawany na pocz�tku, a je�li ju� by� na li�cie to jest przenoszony na pocz�tek
    public bool Add(int v)
        {
            Lista tmp = head;
            if (Search(v)) return false;

            tmp = new Lista(v, head);
            head = tmp;
            return true;
        }

    // Pozosta�e elementy nie zmieniaj� kolejno�ci
    public bool Remove(int v)
        {
            if (head == null)
                return false;
            if (head.value == v)
            {
                head = head.next;
                return true;
            }
            Lista tmp = head;
            while (tmp.next != null)
            {
                if (tmp.next.value == v)
                {
                    Lista p = tmp.next;
                    tmp.next = p.next;
                    return true;
                }
                tmp = tmp.next;
            }

            return false;
        }

    // Wymagane przez interfejs IEnumerable<int>
    public IEnumerator<int> GetEnumerator()
        {
            Lista tmp = head;
            while (tmp != null)
            {
                yield return tmp.value;
                tmp = tmp.next;
            }
        }

    // Wymagane przez interfejs IEnumerable<int> - nie zmmienia� (jest gotowe!)
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
        return this.GetEnumerator();
        }

    } // class MoveToFrontList


} // namespace ASD
