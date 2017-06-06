
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{

public struct ListTestCaseParams
    {
    public char oper;
    public int arg;
    public bool res;
    public int[] elems;

    public ListTestCaseParams(char op, int v, bool r, int[] el)
        {
        oper=op;
        arg=v;
        res=r;
        elems=el;
        }

    }

public class ListTestCase : TestCase
    {

    public IList list;
    public ListTestCaseParams[] data;
    public List<ListTestCaseParams> results = new List<ListTestCaseParams>();

    public ListTestCase(double timeLimit, Exception expectedException, IList l, ListTestCaseParams[] op) : base(timeLimit,expectedException)
        {
        list = l;
        data = op;
        }

    public override void PerformTestCase()
        {
        bool r;
        foreach ( var d in data )
            switch ( d.oper )
                {
                case 'a':
                    r=list.Add(d.arg);
                    results.Add(new ListTestCaseParams(d.oper,d.arg,r,list.ToArray()));
                    break;
                case 's':
                    r=list.Search(d.arg);
                    results.Add(new ListTestCaseParams(d.oper,d.arg,r,list.ToArray()));
                    break;
                case 'r':
                    r=list.Remove(d.arg);
                    results.Add(new ListTestCaseParams(d.oper,d.arg,r,list.ToArray()));
                    break;
                default:
                    throw new ArgumentException($"Nieprawidlowa operacja {d.oper} w przykladzie testowym");
                }
        }

    public override void VerifyTestCase(out Result resultCode, out string message)
        {
        Dictionary<char,string> oper = new Dictionary<char, string>() { ['a']="Add", ['s']="Search", ['r']="Remove" };
        for ( int i=0 ; i<data.Length ; ++i )
            {
            if ( data[i].res!=results[i].res )
                {
                resultCode = Result.BadResult;
                message = $"incorrect result: operation {oper[data[i].oper]}({data[i].arg}), result: {results[i].res} (expected result: {data[i].res})";
                return;
                }
            if ( data[i].elems.Length!=results[i].elems.Length )
                {
                resultCode = Result.BadResult;
                message = $"incorrect elements count: {results[i].elems.Length} (expected: {data[i].elems.Length})";
                return;
                }
            if ( !data[i].elems.SequenceEqual(results[i].elems) )
                {
                resultCode = Result.BadResult;
                message = $"incorrect elements secuence: {results[i].elems.ElementsToString()} (expected: {data[i].elems.ElementsToString()})";
                return;
                }
            }
        resultCode = Result.Success;
        message = "OK";
        }

    }

public class Lab01
    {

    public static void Main()
        {

        // Przyklady testowe dla SimpleList

        TestSet simpleListAdd = new TestSet();
        simpleListAdd.TestCases.Add(new ListTestCase(5, null, new SimpleList(), new ListTestCaseParams[] {
                new ListTestCaseParams('a', 10, true , new int[] { 10 }),
                new ListTestCaseParams('a', 10, false, new int[] { 10 }),
                new ListTestCaseParams('a',  5, true , new int[] { 10, 5 }),
                new ListTestCaseParams('a', 10, false, new int[] { 10, 5 }),
                new ListTestCaseParams('a',  5, false, new int[] { 10, 5 }),
                new ListTestCaseParams('a', 15, true , new int[] { 10, 5, 15 }),
                new ListTestCaseParams('a',  5, false, new int[] { 10, 5, 15 }),
                new ListTestCaseParams('a', 15, false, new int[] { 10, 5, 15 }),
                new ListTestCaseParams('a', 10, false, new int[] { 10, 5, 15 }),
                new ListTestCaseParams('a', 20, true , new int[] { 10, 5, 15, 20 }),
                }));

        TestSet simpleListAddSearch = new TestSet();
        simpleListAddSearch.TestCases.Add(new ListTestCase(5, null, new SimpleList(), new ListTestCaseParams[] {
                new ListTestCaseParams('s', 0, false, new int[] { }),
                new ListTestCaseParams('a', 10, true , new int[] { 10 }),
                new ListTestCaseParams('s', 10, true , new int[] { 10 }),
                new ListTestCaseParams('s', 20, false, new int[] { 10 }),
                new ListTestCaseParams('a', 10, false, new int[] { 10 }),
                new ListTestCaseParams('a',  5, true , new int[] { 10, 5 }),
                new ListTestCaseParams('s',  5, true , new int[] { 10, 5 }),
                new ListTestCaseParams('s', 20, false, new int[] { 10, 5 }),
                new ListTestCaseParams('a', 10, false, new int[] { 10, 5 }),
                new ListTestCaseParams('a',  5, false, new int[] { 10, 5 }),
                new ListTestCaseParams('a', 15, true , new int[] { 10, 5, 15 }),
                new ListTestCaseParams('s', 15, true , new int[] { 10, 5, 15 }),
                new ListTestCaseParams('s', 20, false, new int[] { 10, 5, 15 }),
                new ListTestCaseParams('a',  5, false, new int[] { 10, 5, 15 }),
                new ListTestCaseParams('a', 15, false, new int[] { 10, 5, 15 }),
                new ListTestCaseParams('a', 10, false, new int[] { 10, 5, 15 }),
                new ListTestCaseParams('a', 20, true , new int[] { 10, 5, 15, 20 }),
                new ListTestCaseParams('s', 20, true , new int[] { 10, 5, 15, 20 }),
                }));

        TestSet simpleListAddRemove = new TestSet();
        simpleListAddRemove.TestCases.Add(new ListTestCase(5, null, new SimpleList(), new ListTestCaseParams[] {
                new ListTestCaseParams('r', 10, false, new int[] { }),
                new ListTestCaseParams('a', 10, true , new int[] { 10 }),
                new ListTestCaseParams('r', 15, false, new int[] { 10 }),
                new ListTestCaseParams('a', 20, true , new int[] { 10, 20 }),
                new ListTestCaseParams('r', 15, false, new int[] { 10, 20 }),
                new ListTestCaseParams('r', 10, true,  new int[] { 20 }),
                new ListTestCaseParams('a', 15, true,  new int[] { 20, 15 }),
                new ListTestCaseParams('a', 25, true,  new int[] { 20, 15, 25 }),
                new ListTestCaseParams('r', 15, true,  new int[] { 20, 25 }),
                new ListTestCaseParams('a', 30, true,  new int[] { 20, 25, 30 }),
                new ListTestCaseParams('r', 40, false, new int[] { 20, 25, 30 }),
                new ListTestCaseParams('r', 30, true,  new int[] { 20, 25 }),
                new ListTestCaseParams('a', 15, true,  new int[] { 20, 25, 15 }),
                new ListTestCaseParams('r', 25, true,  new int[] { 20, 15 }),
                new ListTestCaseParams('r', 15, true,  new int[] { 20 }),
                new ListTestCaseParams('a', 10, true,  new int[] { 20, 10 }),
                new ListTestCaseParams('r', 20, true,  new int[] { 10 }),
                new ListTestCaseParams('r', 10, true,  new int[] { }),
                new ListTestCaseParams('a', 15, true,  new int[] { 15 }),
                }));

        TestSet simpleListAddSearchRemove = new TestSet();
        simpleListAddSearchRemove.TestCases.Add(new ListTestCase(5, null, new SimpleList(), new ListTestCaseParams[] {
                new ListTestCaseParams('r', 10, false, new int[] { }),
                new ListTestCaseParams('s', 10, false, new int[] { }),
                new ListTestCaseParams('a', 10, true , new int[] { 10 }),
                new ListTestCaseParams('r', 15, false, new int[] { 10 }),
                new ListTestCaseParams('a', 20, true , new int[] { 10, 20 }),
                new ListTestCaseParams('s', 20, true , new int[] { 10, 20 }),
                new ListTestCaseParams('r', 15, false, new int[] { 10, 20 }),
                new ListTestCaseParams('r', 10, true,  new int[] { 20 }),
                new ListTestCaseParams('a', 15, true,  new int[] { 20, 15 }),
                new ListTestCaseParams('s', 20, true , new int[] { 20, 15 }),
                new ListTestCaseParams('a', 25, true,  new int[] { 20, 15, 25 }),
                new ListTestCaseParams('r', 15, true,  new int[] { 20, 25 }),
                new ListTestCaseParams('a', 30, true,  new int[] { 20, 25, 30 }),
                new ListTestCaseParams('r', 40, false, new int[] { 20, 25, 30 }),
                new ListTestCaseParams('s', 15, false, new int[] { 20, 25, 30 }),
                new ListTestCaseParams('r', 30, true,  new int[] { 20, 25 }),
                new ListTestCaseParams('r', 25, true,  new int[] { 20 }),
                new ListTestCaseParams('r', 20, true,  new int[] { }),
                }));

        // Przyklady testowe dla MoveToFrontList

        TestSet moveToFrontListAdd = new TestSet();
        moveToFrontListAdd.TestCases.Add(new ListTestCase(5, null, new MoveToFrontList(), new ListTestCaseParams[] {
                new ListTestCaseParams('a', 10, true , new int[] { 10 }),
                new ListTestCaseParams('a', 10, false, new int[] { 10 }),
                new ListTestCaseParams('a', 15, true , new int[] { 15, 10 }),
                new ListTestCaseParams('a', 10, false, new int[] { 10, 15 }),
                new ListTestCaseParams('a', 15, false, new int[] { 15, 10 }),
                new ListTestCaseParams('a', 25, true , new int[] { 25, 15, 10 }),
                new ListTestCaseParams('a', 15, false, new int[] { 15, 25, 10 }),
                new ListTestCaseParams('a', 10, false, new int[] { 10, 15, 25 }),
                new ListTestCaseParams('a', 20, true , new int[] { 20, 10, 15, 25 }),
                }));

        TestSet moveToFrontListAddSearch = new TestSet();
        moveToFrontListAddSearch.TestCases.Add(new ListTestCase(5, null, new MoveToFrontList(), new ListTestCaseParams[] {
                new ListTestCaseParams('s', 10, false, new int[] { }),
                new ListTestCaseParams('a', 10, true , new int[] { 10 }),
                new ListTestCaseParams('s', 10, true , new int[] { 10 }),
                new ListTestCaseParams('s', 20, false, new int[] { 10 }),
                new ListTestCaseParams('a', 10, false, new int[] { 10 }),
                new ListTestCaseParams('a', 15, true , new int[] { 15, 10 }),
                new ListTestCaseParams('s', 15, true , new int[] { 15, 10 }),
                new ListTestCaseParams('s', 20, false, new int[] { 15, 10 }),
                new ListTestCaseParams('a', 10, false, new int[] { 10, 15 }),
                new ListTestCaseParams('a', 15, false, new int[] { 15, 10 }),
                new ListTestCaseParams('a', 25, true , new int[] { 25, 15, 10 }),
                new ListTestCaseParams('s', 25, true , new int[] { 25, 15, 10 }),
                new ListTestCaseParams('s', 20, false, new int[] { 25, 15, 10 }),
                new ListTestCaseParams('a', 15, false, new int[] { 15, 25, 10 }),
                new ListTestCaseParams('a', 25, false, new int[] { 25, 15, 10 }),
                new ListTestCaseParams('a', 10, false, new int[] { 10, 25, 15 }),
                new ListTestCaseParams('a', 20, true , new int[] { 20, 10, 25, 15 }),
                new ListTestCaseParams('s', 25, true , new int[] { 25, 20, 10, 15 }),
                }));

        TestSet moveToFrontListAddRemove = new TestSet();
        moveToFrontListAddRemove.TestCases.Add(new ListTestCase(5, null, new MoveToFrontList(), new ListTestCaseParams[] {
                new ListTestCaseParams('r', 10, false, new int[] { }),
                new ListTestCaseParams('a', 10, true , new int[] { 10 }),
                new ListTestCaseParams('r', 15, false, new int[] { 10 }),
                new ListTestCaseParams('a', 20, true , new int[] { 20, 10 }),
                new ListTestCaseParams('r', 15, false, new int[] { 20, 10 }),
                new ListTestCaseParams('r', 10, true,  new int[] { 20 }),
                new ListTestCaseParams('a', 15, true,  new int[] { 15, 20 }),
                new ListTestCaseParams('a', 25, true,  new int[] { 25, 15, 20 }),
                new ListTestCaseParams('r', 15, true,  new int[] { 25, 20 }),
                new ListTestCaseParams('a', 30, true,  new int[] { 30, 25, 20 }),
                new ListTestCaseParams('r', 40, false, new int[] { 30, 25, 20 }),
                new ListTestCaseParams('r', 30, true,  new int[] { 25, 20 }),
                new ListTestCaseParams('r', 25, true,  new int[] { 20 }),
                new ListTestCaseParams('r', 20, true,  new int[] { }),
                }));

        TestSet moveToFrontListAddSearchRemove = new TestSet();
        moveToFrontListAddSearchRemove.TestCases.Add(new ListTestCase(5, null, new MoveToFrontList(), new ListTestCaseParams[] {
                new ListTestCaseParams('r', 10, false, new int[] { }),
                new ListTestCaseParams('s', 10, false, new int[] { }),
                new ListTestCaseParams('a', 10, true , new int[] { 10 }),
                new ListTestCaseParams('r', 15, false, new int[] { 10 }),
                new ListTestCaseParams('a', 20, true , new int[] { 20, 10 }),
                new ListTestCaseParams('s', 10, true , new int[] { 10, 20 }),
                new ListTestCaseParams('r', 15, false, new int[] { 10, 20 }),
                new ListTestCaseParams('r', 10, true,  new int[] { 20 }),
                new ListTestCaseParams('a', 15, true,  new int[] { 15, 20 }),
                new ListTestCaseParams('s', 20, true , new int[] { 20, 15 }),
                new ListTestCaseParams('a', 25, true,  new int[] { 25, 20, 15 }),
                new ListTestCaseParams('r', 15, true,  new int[] { 25, 20 }),
                new ListTestCaseParams('a', 30, true,  new int[] { 30, 25, 20 }),
                new ListTestCaseParams('r', 40, false, new int[] { 30, 25, 20 }),
                new ListTestCaseParams('s', 15, false, new int[] { 30, 25, 20 }),
                new ListTestCaseParams('r', 25, true,  new int[] { 30, 20 }),
                new ListTestCaseParams('r', 20, true,  new int[] { 30 }),
                new ListTestCaseParams('r', 30, true,  new int[] { }),
                }));

        // Testy dla SimpleList

        Console.WriteLine("\n*** Test 1.1");
        Console.WriteLine("SimpleList - Add");
        simpleListAdd.PreformTests(verbose:true, checkTimeLimit:false);

        Console.WriteLine("\n*** Test 1.2");
        Console.WriteLine("SimpleList - Add,Search");
        simpleListAddSearch.PreformTests(verbose:true, checkTimeLimit:false);

        Console.WriteLine("\n*** Test 1.3");
        Console.WriteLine("SimpleList - Add,Remove");
        simpleListAddRemove.PreformTests(verbose:true, checkTimeLimit:false);

        Console.WriteLine("\n*** Test 1.4");
        Console.WriteLine("SimpleList - Add,Search,Remove");
        simpleListAddSearchRemove.PreformTests(verbose:true, checkTimeLimit:false);

        // Testy dla MoveToFrontList

        Console.WriteLine("\n*** Test 2.1");
        Console.WriteLine("MoveToFrontList - Add");
        moveToFrontListAdd.PreformTests(verbose:true, checkTimeLimit:false);

        Console.WriteLine("\n*** Test 2.2");
        Console.WriteLine("MoveToFrontList - Add,Search");
        moveToFrontListAddSearch.PreformTests(verbose:true, checkTimeLimit:false);

        Console.WriteLine("\n*** Test 2.3");
        Console.WriteLine("MoveToFrontList - Add,Remove");
        moveToFrontListAddRemove.PreformTests(verbose:true, checkTimeLimit:false);

        Console.WriteLine("\n*** Test 2.4");
        Console.WriteLine("MoveToFrontList - Add,Search,Remove");
        moveToFrontListAddSearchRemove.PreformTests(verbose:true, checkTimeLimit:false);

        }

    }

}