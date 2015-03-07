# Contributing Guidelines

Issues, pull requests, and other contributions are welcomed!

A few tips:

## Is `OSVR-Unity` the right repository?

The OSVR system consists of a number of repositories, many of which are in the [OSVR organization on GitHub][osvr-org].

The `OSVR-Unity` repo consists of two parts: the Managed-OSVR bindings for using OSVR ClientKit from .NET-based languages, and the OSVR-Unity integration and sample code.

- If you've got an issue to report, a bug fix, or a feature addition to those components: **yes**, you're in the right spot!
- If you have an issue, bug fix, or other contribution related to the OSVR server (which comes bundled with an OSVR-Unity binary snapshot) or one of the plugins: **you're close** - see the [OSVR-Core](https://github.com/osvr/OSVR-Core) repo for the server and its bundled plugins.
- If you have a hardware or software issue related to OSVR but aren't sure exactly where it fits: **let us know with a support ticket at <http://support.osvr.com>**

[osvr-org]: https://github.com/osvr

## Getting ready

General "getting started" instructions can be found at <http://wiki.osvr.com>.

When making pull requests, please fork the project and create a topic branch off of the `master` branch.
(This is what GitHub does by default if you start editing with your web browser.)

When developing, make small commits that are nevertheless "whole": small enough to review, but each containing a logical single change in its entirety.
(If you don't understand what we mean by this, that's OK, we'll work it out.)

It's OK to rebase your topic branch to make the history more clear.
Avoid merging from master into your topic branch: if you need a change from master, rebase; otherwise, try to keep topic branches short-lived enough that we can get your code in to the mainline before much else changes!

Try to develop code that is portable (not particularly tied to a single operating system/compiler/etc) - OSVR runs on a number of platforms, and while we don't expect you to have all of them to test on, it's good to keep in mind.

Note that the Managed-OSVR code should be entirely Unity-independent, so it can be used in other .NET-based applications. The reason for the specific version of the .NET framework it is using is for compatibility with the runtime used by Unity.

If you're adding something reasonably testable, please also add a test.
If you're touching code that already has tests, make sure they didn't break.

Please follow general "idiomatic practices" with regards to code style.
The main points are to match code surrounding what you're edited.
These help ensure that your changes are not artificially large because of whitespace, etc, that it's easy to review your changes, and that your code will be maintainable in the future.

Keep in mind that some changes have the potential to impact (that is, break) a lot of downstream code (other integrations, games), so changes that affect how a developer would write code using these bindings will be more rigorously reviewed and discussed.
We'd recommend opening an issue for discussion before you start work on any such changes, to make sure we'll be able to accept your contribution.

## License

No formal copyright assignment is required. If you're adding a new file, make sure it has the appropriate license header. Any contributions intentionally sent to a project are considered to be offered under the license of the project they're sent to.
