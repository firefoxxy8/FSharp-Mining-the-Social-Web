﻿module Search

open System
open System.Linq
open LinqToTwitter
open TwitterContext

// get search result
let getSearchResult q num = 
    query { 
        for searchResult in ctx.Search do
            where (searchResult.Type = SearchType.Search)
            where (searchResult.Query = q)
            where (searchResult.Count = num)
            select searchResult
            exactlyOne
    }

// get search result with specific maxId
let getSearchResultWithMaxId q num maxId = 
    query { 
        for searchResult in ctx.Search do
            where (searchResult.Type = SearchType.Search)
            where (searchResult.Query = q)
            where (searchResult.Count = num)
            where (searchResult.MaxID = maxId)
            select searchResult
            exactlyOne
    }

// get statuses from number of batches
let getStatuses q num batches = 
    let s2ul (s : string) = Convert.ToUInt64(s)
    
    let getStatuses q maxId = 
        (getSearchResultWithMaxId q num maxId).Statuses
        |> List.ofSeq
        |> List.rev
    
    let combinedStatuses (acc : Status list) _ = 
        let maxId = 
            if acc = [] then UInt64.MaxValue
            else 
                (acc
                 |> List.head
                 |> (fun s -> s.StatusID)
                 |> s2ul) - 1UL
        (getStatuses q maxId) @ acc
    
    [ 0..batches ] |> List.fold combinedStatuses []
