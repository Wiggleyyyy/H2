"use client";
import { useState } from "react";
import Career from "@/components/Career";
import Tournament from "../components/Tournement";

export default function Home() {
  const [results, setResults] = useState([]);

  const addResult = (result) => {
    setResults((prevResults) => [...prevResults, result]);
  };

  return (
    <main className="flex flex-row h-[100%] w-[100%]">
      <Career results={results} />
      <Tournament onAddResult={addResult} />
    </main>
  );
}
