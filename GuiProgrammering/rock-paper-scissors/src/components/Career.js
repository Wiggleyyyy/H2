"use client";

import { Separator } from "./ui/separator";
import { useState } from "react";

export default function Career({ results }) {
  return (
    <div className="w-[25%] h-[100%] bg-neutral-900">
      <div className="flex flex-col justify-center items-center p-5">
        <h1 className="text-2xl">Recent Games</h1>
      </div>
      <Separator />
      <div className="flex flex-col h-[85%] justify-start items-center p-5">
        {results.map((result, index) => (
          <div key={index} className="flex flex-row justify-evenly items-center w-[80%] bg-neutral-950 p-2 m-2">
            <h2>{result.players[0]}</h2>
            <h2>{result.scores[0]} - {result.scores[1]}</h2>
            <h2>{result.players[1]}</h2>
          </div>
        ))}
      </div>
    </div>
  );
}
