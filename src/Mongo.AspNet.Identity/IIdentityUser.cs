﻿/*
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
    public interface IIdentityUser
    {
        string Id { get; set; }
        string Email { get; set; }
        string PhoneNumber { get; set; }
        string PasswordHash { get; set; }
        string SecurityStamp { get; set; }
    }
}