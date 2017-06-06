
using System;
using System.Collections.Generic;

namespace ASD
{

public interface IList : IEnumerable<int>
    {
    // Jeœli element v jest na liœcie to zwraca true
    // Jeœli elementu v nie ma na liœcie to zwraca false
    bool Search(int v);

    // Jeœli element v jest na liœcie to zwraca false (elementu nie dodaje)
    // Jeœli elementu v nie ma na liœcie to dodaje go do listy i zwraca true
    bool Add(int v);

    // Jeœli element v jest na liœcie to usuwa go z listy i zwraca true
    // Jeœli elementu v nie ma na liœcie to zwraca false
    bool Remove(int v);
    }

//
// dopisaæ klasê opisuj¹c¹ element listy
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
// Zwyk³a lista (nie samoorganizuj¹ca siê)
public class SimpleList : IList
    {

        // dodaæ niezbêdne pola
        public Lista head = null;
    // Lista siê nie zmienia
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

    // Pozosta³e elementy nie zmieniaj¹ kolejnoœci
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
            // nie wolno modyfikowaæ kolekcji
            Lista tmp = head;
            while(tmp!=null)
            {
                yield return tmp.value;
                tmp = tmp.next;
            }
       // zmieniæ
        }

    // Wymagane przez interfejs IEnumerable<int> - nie zmmieniaæ (jest gotowe!)
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
        return this.GetEnumerator();
        }

    } // class SimpleList


// Lista z przesnoszeniem elementu, do którego by³ dostêp na pocz¹tek
public class MoveToFrontList : IList
    {

        // dodaæ niezbêdne pola
        public Lista head = null;

    // Znaleziony element jest przenoszony na pocz¹tek
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

    // Element jest dodawany na pocz¹tku, a jeœli ju¿ by³ na liœcie to jest przenoszony na pocz¹tek
    public bool Add(int v)
        {
            Lista tmp = head;
            if (Search(v)) return false;

            tmp = new Lista(v, head);
            head = tmp;
            return true;
        }

    // Pozosta³e elementy nie zmieniaj¹ kolejnoœci
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

    // Wymagane przez interfejs IEnumerable<int> - nie zmmieniaæ (jest gotowe!)
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
        return this.GetEnumerator();
        }

    } // class MoveToFrontList


} // namespace ASD
