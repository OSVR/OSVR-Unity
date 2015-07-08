/// OSVR-Unity Connection
///
/// http://sensics.com/osvr
///
/// <copyright>
/// Copyright 2014 Sensics, Inc.
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///     http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// </copyright>


namespace OSVR.Unity
{
    /// <summary>
    /// Common .net interface for the set of Requires*Interface base classes.
    /// </summary>
    /// <typeparam name="T">The report/state type of the OSVR interface.</typeparam>
    public interface IRequiresInterface<T>
    {
        OSVR.ClientKit.IInterface<T> Interface { get; }
    }
}
