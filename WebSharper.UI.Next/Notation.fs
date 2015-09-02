// $begin{copyright}
//
// This file is part of WebSharper
//
// Copyright (c) 2008-2014 IntelliFactory
//
// Licensed under the Apache License, Version 2.0 (the "License"); you
// may not use this file except in compliance with the License.  You may
// obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
// implied.  See the License for the specific language governing
// permissions and limitations under the License.
//
// $end{copyright}

namespace WebSharper.UI.Next

open WebSharper

module M = WebSharper.Core.Macros
module Q = WebSharper.Core.Quotations
module R = WebSharper.Core.Reflection
module C = WebSharper.Core.JavaScript.Core

module Notation =

    [<Sealed>]
    type GetValueMacro() =
        interface M.IMacro with
            member this.Translate(q, tr) =
                match q with
                | Q.CallModule (c, [ o ]) ->
                    let t = c.Generics.Head.DeclaringType
                    Q.PropertyGet(
                        {
                            Generics = c.Generics
                            Entity = R.Property.Parse (t.Load().GetProperty "Value")
                        }, [o])
                    |> tr
                | _ -> failwith "GetValueMacro error"

    [<Sealed>]
    type SetValueMacro() =
        interface M.IMacro with
            member this.Translate(q, tr) =
                match q with
                | Q.CallModule (c, [ o; v ]) ->
                    let t = c.Generics.Head.DeclaringType
                    Q.PropertySet(
                        {
                            Generics = c.Generics
                            Entity = R.Property.Parse (t.Load().GetProperty "Value")
                        }, [o; v])
                    |> tr
                | _ -> failwith "SetValueMacro error"

    [<Macro(typeof<GetValueMacro>)>]
    let inline ( ! ) (o: ^x) : ^a = (^x: (member Value: ^a with get) o)

    [<Macro(typeof<SetValueMacro>)>]
    let inline ( := ) (o: ^x) (v: ^a) = (^x: (member Value: ^a with set) (o, v))

    [<JavaScript; Inline>]
    let inline ( <~ ) (o: ^x) (fn: ^a -> ^a) = o := fn !o

    [<JavaScript; Inline>]
    let inline ( |>> ) source mapping = View.Map mapping source

    [<JavaScript; Inline>]
    let inline ( >>= ) source body = View.Bind body source

    [<JavaScript; Inline>]
    let inline ( <*> ) sourceFunc sourceParam = View.Apply sourceFunc sourceParam
