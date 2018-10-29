| branch | build status |
|---|---|
| `master` | [![Build Status](https://travis-ci.org/Stephanvs/Mingle.svg?branch=master)](https://travis-ci.org/Stephanvs/Mingle) |
| `dev` | [![Build Status](https://travis-ci.org/Stephanvs/Mingle.svg?branch=dev)](https://travis-ci.org/Stephanvs/Mingle) |


# What is a CRDT?

Conflict-free, Coordination-free, Commutative, or Convergent datatypes, CRDT's are usually formally described as "join semi-lattices". Mathematical jargon aside, CRDT's track causality for modifications to your data. Because of this, time becomes less relevant, and coordination becomes unnecessary to get accurate values for your data.

# Want to learn more?

Here are some resources that may help you understand further.

- Strong Eventual Consistency and Conflict-free Replicated Data Types
    - A good introduction to the concept of CRDTs: http://research.microsoft.com/apps/video/default.aspx?id=153540&r=1
- A comprehensive study of Convergent and Commutative Replicated Data Types
    - A survey with references for several popular CRDTs: http://hal.inria.fr/docs/00/55/55/88/PDF/techreport.pdf
- Efficient State-based CRDTs by Delta-Mutation
    - Talk: https://www.youtube.com/watch?v=y_ewFP-lgyM
    - Paper: http://arxiv.org/pdf/1410.2803v1.pdf