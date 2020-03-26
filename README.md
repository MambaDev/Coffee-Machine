# Coffee-Machine

A small coffee machine application with rest api.

### Notes

- Bug related to not being able to descale had to be fixed, since you could only descale in a Okay state, not as described in the task document.

- The random number assignment will never hit the value since its within the value and not inclusive.

### Assumptions

- Per day of the week show the time the first cup and last were made: This is over the life span of the machine and not just the last week.

- First and last cup are relative to the life time and not just the latest time for that day.
