/// Copyright (C) 2012-2014 Soomla Inc.
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///      http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.

namespace Project_Data.Scripts.Json
{
	/// Extend this class if you want to use the syntax
	///	<c>if(myObject)</c> to check if it is not null
	public class NullCheckable {

		public static implicit operator bool(NullCheckable o) {
			return (object)o != null;
		}
	}
}
