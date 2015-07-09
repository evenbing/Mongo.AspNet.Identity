/*
    Copyright 2015 Matías Fidemraizer (https://linkedin.com/in/mfidemraizer)
    
    "Mongo.AspNet.Identity" project (https://github.com/mfidemraizer/Mongo.AspNet.Identity)
    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
 
    You may obtain a copy of the License at
    http://www.apache.org/licenses/LICENSE-2.0
    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

namespace Mongo.AspNet.Identity
{
    using Microsoft.AspNet.Identity;
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;

    public sealed class ExtendedUser
    {
        public ExtendedUser()
        {
            Roles = new HashSet<string>();
            Logins = new HashSet<UserLoginInfo>();
        }

        public string Id { get; set; }
        public HashSet<string> Roles { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool TwoFactorAuthenticationEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset LockoutEndDate { get; set; }
        public HashSet<UserLoginInfo> Logins { get; set; }
        public HashSet<Claim> Claims { get; set; }
    }
}