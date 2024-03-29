<p align="center">
  <img 
    src="http://83.212.120.119/images/logo_fixed.PNG" 
    width="125px"
  >
</p>

<h1 align="center">Smart Transit for NEO</h1>

<p align="center">
	A powerful, decentralized, transparent ticketing & logistics solution
	for national and state-owned transit systems in the EU.
	Powered by the <b>NEO blockchain</b>.
</p>

- [Overview](#overview)
- [How Smart Transit works](#how-smart-transit-works)
- [Problems we faced](#problems-we-faced)
- [Our TODO List/Roadmap](#our-todo-list/roadmap)
- [Aknowledgements & License](#aknowledgements-&-license)

---

## Overview
Smart Transit was developed in European Public Transit in mind, be it buses, trains or the metro.
It is a Blockchain-based ticketing and logistics solution that benefits everyone - The customer that buys tickets fast and hassle-free, to the Transit Organization that manages the actual transit and receives 
live market data from the system, all the way up to the Governing Body that regulates Public European Transit, and wishes to audit the system regularly and transparently.
We discuss the specific merits for every group (The commuter, the Transit Organization/Corporation, and the Government) below.

### How Smart Transit works

__1. Customer-friendly__
  - Transportation is fully in the hands of the commuter.
  - Customers interact with the NEO blockchain and only the NEO blockchain, to buy tickets through our C# Smart Contract
  - Users are incentivised (through a 50% refund) to mark their trip as "complete" when they disembark, providing live marketing data with small error margins to the Transit Organization.
  - As we respect the Right to Privacy of a European citizen, at no point does our commuter yield his private details, as it is not required - the system does not rely on trust.
  

__2. Transit-Organization-Friendly__
  - Efficient ticketing without printing and logistical costs
  - Live Marketing data about your transit lines (such as current Active tickets, popularity) without monitoring your passengers. Instead, your passengers provide the information themselves, with respect to their own rights
  - The whole infastracture is decentralized, which means away from your own hands and not vulnerable to attacks and downtime.
  - (NOTE: Organizations are strongly encouraged to run a Node on the NEO Network, and give back to the community, which provides the Blockchain at no cost)
  
  
__3. Government-friendly__
  - The entire system is completely transparent and ready for auditing by any governing body, to ensure the Transit Organization's fair treatment of European public funds and ticket income.
  - Dishonesty and falsehoods are, by design, unable to exist on the NEO Blockchain, and therefore ruled out.
  
### Problems we faced
A general lack of documentation is what held us back the most. Out-of-date docs and buggy main branches on various `neo/CoZ` projects was our main source of frustration, but at the same time, that frustration familiarized us with the process of building an Open-Source project based on NEO. With every problem, we learned that we need to post an Issue on GitHub. With every trouble, we learned that we need to discuss the issue with others on the StackExchange (while we had it) or on Discord. With every breakthrough and solution, we found out that the correct thing to do is make a pull request and share the solution with all other NEO devs. In the end, the feeling of community and belonging that came after overcaming these difficulties is something as important to us as the project itself.

### Our TODO List/Roadmap

- TicketManager Smart Contract (`Ready and deployed`)
- Live Demo using 2 virtual buses and 3 virtual passengers (`In progress - Debugging`)
- Account database on site, that creates a new wallet for each user when they register (`Not started - In Design`)
- HTTPS implementation on smarttransit.eu, for encrypted and secure data transfers (`Done!`)
- Use of SmartTransit token instead of GAS (`Abandoned - Not needed, GAS is as good as a new token in this case - Not every idea needs an ICO`)

### Aknowledgements & License
- Big thanks are owed to `relfos`, author of the `neo-debugger-tools` repo, for all the help and guidance he provided.
- Another big thanks to users @SamGuy, @jwhollister and @MichaelHerman of the `#csharp` channel on NEO's Discord, for answering our questions despite the channel's lack of traffic. Without you, we'd have stalled long ago.
- A general thanks to the whole NEO Dev team, for providing the tools through which we made our idea a reality.

Website: https://smarttransit.eu

Created by: Dimitris Sfounis (<https://dsfounis.com/>),
Theocharis Iordanidis,
Stefanos Dragoutsis

Greece, March 10th 2018

This project is released under the MIT license, see the `LICENSE` file for more details.
