﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.9.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "899e57b91ad1c27962ce715ffcbad5acd66012c3cd91150e1a6821c8523b982498513ebeaa0baf1456c9d4c6aea4e997472c0249fae4ec58b5cfd1945f383269" },
                { "ar", "d35d2859c7de27d195b3898cb25c66484036181e2db5c5e32236982c81eabbe7e3d34ed31c5f5145a9df2347253216673e6a54d01f7998fa778a151bdcfa8bea" },
                { "ast", "57c0360810ec1a6f574374b1405ab6feae83063a2c56f3a10738bc506e6aecf783cd37b14ad6fc231ae517f656e4f366ef1afa2eb263a349caaa248354997669" },
                { "be", "31443dcd3cf0467dc682f272e18c611562ea74271f35eed1054a5bd0ced244ccb499feabfd63c8b7c21e3994a2291c0ea11b6abc6abdeca066b5c7840b069f07" },
                { "bg", "3655d91d9291ce0f506281e3d8589423ede4a8d9e90f7c7266f28d9cd284808abfe70ade018cbdb6166d7b21e8dfe69aa392a837b0d441888f5d45aa13c7806d" },
                { "br", "75d4f15ddad3f81a0a43d970c3f3ce5e4e409c123c690a91b7adb141f0e6f1a88cfeeed130f21f7a2e977fba851d55a034c30222eeb72f4af1bac68816acda81" },
                { "ca", "0f9dcd5129c319affdee07d695b8a82595cbddb99e9e198c541ad1840034a7e017e5ff9175b3d941302550f8f63369e4383768d16dbec46a34e678874cb2d267" },
                { "cak", "ab80932197fd437ce43bef5b239352db9fb20a54f81b30670e0c3e12b83305ec19a0c296ada0d36e70c0b47786def6f3fc976b5efa75f5737adb6f259afae8eb" },
                { "cs", "93ad67aa67013cfc5935d1104a67b21ecca01a51f7e0fd9d2418f4bfbb8bcf3e3daef88f97f8022b39fb7b82aceacaeaad7b41211a8829e4750152861a5ba74c" },
                { "cy", "05df1b922a5b7993d1252f290a0e30b35dc89ba94bb4b48ee5baba4a94d9f1d2a4516fb3cfbb611bb9536b695b5dce99adc4da89d733c637f99c5ef1cb0f3639" },
                { "da", "7d9d9977945e2a7d8360a9199c8eac99fe7882d00bc924a8aa10947a6d537dc7c38f9a4bfc5281922702db49e1a820f27188515d8f33bc566e1b2c1ec807845c" },
                { "de", "d9eb21dbb6fb4d3d20c051698362d70683f9dcbc95cb2cea287481b994e715a79ecb3115f136b9fc632a32edead9da3585554bab6c72202db9bdaded46336e74" },
                { "dsb", "3f72270b8ed243dd839a25ef8e3447e826a7611e28728b81fdd8cc2e4f6dcb755379d22c7c717ac1e1bd85853aa1ed9e22c47336d9b0919a0543bd4479235463" },
                { "el", "55c11aa6efd631b8bb72d1004de3af5832e0c48c09d978f246bcd51d89257bf39f2abcf9771635b5923f8030e9fa8369109d71392e47048c5c83be0927601bf9" },
                { "en-CA", "9426aa60b93439b27aef9c2b6e569632ef92584e1914f3ed83b26e90e18db0423eec3d9593c4b482fb8266fb146261628850978533bb6eac7cbf9cac24b4045a" },
                { "en-GB", "47be8bf2cf6d02281de8ef54e33519bbb282a6aff23fbbeb6ad12db1f4cf2cf68afb49a788dc426618ce65dff48391c00439c59b140bfaa76e6373133f7f1d38" },
                { "en-US", "b314bff1b7a426c4d3029c18b9097cdaf7c46e4ee4625f907a3d7e65e3eea1925e1d9c0057cabd84494cb351b7b1c9ccca5236704757f8fecfb22c551740d61d" },
                { "es-AR", "9f18fc53f65bd95031450276aee1a695bf9dafccceac6febedcd69ec25a125ecdd5943d093d356c68bda5f85aab7258de7a5f4901a4191935148eee64b50f1a6" },
                { "es-ES", "53a815af318add332ed6b8310d38d9d231a54fd7e28f9e39c9e42cf58e6462bf7e729b6ac87bc6def5a17533cdf3513ecac3f7f7967226611facd99b53f8dac0" },
                { "es-MX", "a3ecdbd59632006b1cf06cd13982d5cf7224c3af7738af349042796fd05d92a67bb5576553c07b0f9a3e3eb1bce9d6b5de7cccf7203bcbd0683470d32b7b1506" },
                { "et", "6290466c9eb31f16396db29a670ae92a6df030ef313c0ae8ee8a74051d83c457caa6be3ba620e21850a0798720615a54c07239d7b3b78239c771980514872e70" },
                { "eu", "8b193cc334845ca27b996bd16c5bc536e90f5ede24b5fac7623af021fcf35d9a0416aaf7119b7bba138967a41e010ab44171e11ba9095cc0c653cb065017ede7" },
                { "fi", "11896705833b20d6c57683d7b7ec1955a0e1cb8c157f999bd22f065756b9e23ffb826b57df695ecfdb27b445ee77e177dd26ed1481d3a9b60e832781dc22eff2" },
                { "fr", "2a57072804cdf3b4e3f5fd8f559bbf426f941fb4d2efe7fa75e3c29ac762b762e72d424b5bacf127ca93ab84219a5c5900c02a97d7b4bfb75a7b746163e14da2" },
                { "fy-NL", "6c6970210112afa8322794ce75a48f879795aee4f6e2456934f01f054e484498dc06b3f375361592d331a6dff2b041cd1f22424aea0314ea24e4670869d89c2d" },
                { "ga-IE", "39040ad996a5e2730283ca35dd7307529705585f32af388cc3551c74dcdd481edf559fe538ba7fba22fbab0ae7920c703e1e34a114d82b1d2e5755a2bd8ae048" },
                { "gd", "6905c7f08a094ed808aacb892f27573be0decdf814401e4215edd9bbc6015bb7c26935a57f4ade15c0bd32531709a0cf25030082b0ee67ad62c7602f0fad8faf" },
                { "gl", "6d7cbadd50158f02f47d8a6b1dfa8d239a1cf6fd4a3f7bf821b246dfa1f50f2f22b299f625475c7a39a719622cb850661df8816cbf2069c20169248b5f88043e" },
                { "he", "1c42bf637895f1c4605d57a74efe1185e2842df0c4554ece4111c8777531c48b4ebe1e7cf782ccf150950009fbd6c4d224062601fbdd70a2b52529917be43b02" },
                { "hr", "be65b7909f9e26c7d622ac393114fe2baf29e0e9fbca6b8c0d890376e78a17368b3c33d2e749872d7a4d9cc37882b2d59a6db7dceccc637a4ee446743f243750" },
                { "hsb", "264f87752d7d255408954ef76cabd620d709eeaaf907ec5d214437f57e817fe561786295e9e7349e70e449721281b450c329db26b0384a1a049d6b0fcaf45147" },
                { "hu", "89fb229cc2cbeee5ad723d0090be74d91e7a7154cb476360950d4b8588814628c5e0112d4f6acfa97ec0802bb8bd95b0701ae26817f3e5ba4b7124b03d04dbdb" },
                { "hy-AM", "c3e19b848c85e984b8249b82f27ec2c6f448ceb7fd92dca9756155a57d8c0f9ff054e6cc1bf84084af683ea3ea911f789734eab5173c02eb783c3203d02ef387" },
                { "id", "560c9bd55a681749b412343b465ff2480a54ee6905440cdb8dc96ae14ac872e7cd2224f2d8452aecd191c207a3030b14068fd539e73020065c31a2e222de8f53" },
                { "is", "e94dfa1eeb715685e2c224a0b29640f1e59343546f10d8a881a9123812826a86853e832281b9a649d185b6fec028cf799a611b91ec62eb7481a5a16b11737942" },
                { "it", "26f4f0016ca5f7ef46a29485e9f00699d70bacb3cd35a8ab0397e5af4061853fd92b1e5b4cb2f4c1fce96ae6bf931b4de042d41a3b570449af48ff2fb7fb7da2" },
                { "ja", "451bb46ae7ac33faee8235e57f7313e9cdf13996897efae3cad6c2366219c02b6b0f7e1d854c40010dc965d5e26e3622ed98778eaca179180319fc00911b6ae9" },
                { "ka", "3541285be9174d789b9ca8b4313976f985b1f33e527860717dba3cc3899a383bae3a4d32b47e588d88248042b08dbc82cd16a72d269fd9a0747798c00c03923e" },
                { "kab", "eaafbeb8cb095ddc07f71685def94fcfea33d9ace0d5858a0904a07bc36e9d17fa517ad8825fa2071a857bc5e94ec3773c4a5ef419285500060c6f11e89d381f" },
                { "kk", "22dce8e2490b3c006f924399a032806ee67a81fd927abe4455ad0c2c47706e994cf07228ac567d67b4c58488dbb66937b599907f3957a8a456f7121c92cfd7bb" },
                { "ko", "423a52338da34232e9f5c667c17b0869e8dd6c69ffdb909d9942ad4ef18799be96d2975414b63635f29297e1dbc3ffe9587f144993e13ebdd0df52d7332dbcc3" },
                { "lt", "71af2fb8f2bddd865eb5bda34a196b2337c55bc69bca3b09fa1ef069c908e83a953bc30e7772772bab9193c69b9f6feb22e25549516ea0c6348df66cc45f2ad5" },
                { "lv", "eccd1e4b240c34b840db6e0c9898fa4baa140296addc2cb70fd11a60d486fa4e5eee31e889c6f0ab4812cbbe661ba34b13f43a7d42a06a7815493abdbbc0f1ca" },
                { "ms", "69c75f43bfb1e352e053e5370b96d21566d089c8f2ec97828e75678c9e2cb74a4f0608e557b11e5dfa510670c2c8ec5b623e99e50e793480eff73e5a33ce471a" },
                { "nb-NO", "e6c312db8a5487e055b213f1988d53507fda22efb56f3dc2979211b7385551828994bcab24555ebdb0c6e2f4a6e75a636955b956987192a11558876ad67fc052" },
                { "nl", "f22c64a3e9a6cf3bd1511f6c95075d36582c183683cb235e42aef494a773de63bd7c1324a0596a1802448ce5796c112e38259636a5fd13784f6b49b534f82609" },
                { "nn-NO", "d609c25523a59d5f0c54c690f3fa88de338ff4053e2cd01d70cc5f2464650969c741e54f3696749b236be9233d5fc93de1983ff5e1f2ac50315ec1c3af7a6e24" },
                { "pa-IN", "1216360b74e3d5dc847293a51863e86dd13ab0192f424acaf33647688c88c0f6ddbb553723fbc421a6c01e431b8148b07efc9f1b89913d60a06f62480ef2c6e3" },
                { "pl", "364f82f5b552d83ef58a8b20f606a27e694e9de04fc0f113a2d9812365c6baa710659fc49073e5bf66f55517d8b0e6d46a1cbfc0bd1a37f5f0223f57342ba005" },
                { "pt-BR", "f03a8b4b6be3beabcabdf1f93f478ed43f92474dbd24f4fb1ad98fbe22c8dc5a62f1311a0b9663e14ddafaed43e3ef132794288c18330729f2a419160ced8e83" },
                { "pt-PT", "fb1901e886738313eb7145689c7014489d93cd9a51df49de783575e6fb11622dc302fe0e9d8e3c9625945dbf83cef848d5e26301ab4f41fb5c8517426587edd0" },
                { "rm", "6efede55bc84bc15fe73f8033e1371736168b37b769744a7f88fcf709f01b57b3cd2da63aa50634117b2d661ce99e70198ce6598af2df2f9294cdf7f32090f58" },
                { "ro", "5f5c2f4dec29b836c3ef57a44c3ce142168b754a08e8f455eb910c4420759b62e75514e3f81d14a23f60679f7046d4a5673a28958e5bb95436ca29025d14e6c0" },
                { "ru", "2a7f92e231fe49887dc99a0e9fb3dedc4e2af5274d2c261b2672d278b2e2309010f811bd94ad63fcbbf5aa3c275a9411cc12228c1c2c9bdbfc738d79252864e6" },
                { "sk", "3f20ea5ee9c0dc5005c0221dc1f9fc543b421eeff34658b0451ab2a5e3cea5d07e5dbaf5a1fbce96c087d35c18b8699abf18c62b32845a7092b90958098cacab" },
                { "sl", "6600ee6f885b3fab9bbb8591d88c48222f027f2637e05b73b7e7471020c4de95f77f3e7dc7c85121ef4fb8c85f49d2f878991d6701b7de9d1c6838383c2b9fdf" },
                { "sq", "080ee2991bd6d5357bd8822471bff56037d02d985301c0f633694103f1bb25fa40fdac5c313ff9dcd0b5da03e83f6744f935a83e87c6e8e8cd76962cbf52e623" },
                { "sr", "07dd5f34c1cd2d35bc9ef341ddf9c5571b5e553d5623fe64b1440f522beac1eacac96a32b8b0f0cb83de3d268b75e8540fc8b53bd602adddbb0fc99c2db61340" },
                { "sv-SE", "77a2619657f5ef1e9e36506a5a3cb1b77a0d27af13dd541ae7bdee842aa30807aa394036ec4d630ec925cfb59877abdbaeca134b32de3d6f1c6b051153aa4f9e" },
                { "th", "62f42acbc90f27600c459f746759450c4ca7a6aa847a3a74dc01527b82c2943af40ede5ec39d69e5f79da8337ef4623f3d9fbe131422a6566558e05c871bbc5d" },
                { "tr", "e9193dd0bd3af33da81c39b9fd85f42676e63ab067f44bf31e52a3a4b60590f7e8e10c71cd117a777d9b0ae329ff7bbeff197f125526ee34e269613823d74a75" },
                { "uk", "a30c5c079fc6a6215ea6c8fd7c5e788ace09d2129657429e3ebf4e4d57537911683d3818a35e77a3513ed29bc806f226f10c3e836a9526fd02b1bf0890fec288" },
                { "uz", "ed5c2844b017ac6b82faa681881543366ea76fc379e678d3e9d4a0a758d0dcd1a4e2314bdf91b5da175068ad3387d87a2c740d385191c136ea3e6499c61e23f5" },
                { "vi", "c8db1be12e48cd7e0ed3f8c12daaf52f65544af6d1ac44be720849493f3fd6886d767bd65ef5452ab45f920d42289653d3761dce0bdce48d3cb4d6f42b3db446" },
                { "zh-CN", "00175be128e092e778e7f87ef7a9f821639cc39ed44878955b80971cfa5386d41fd5984c0ac2b124e38a1dd6b7e8607dcef342a736ef2749a6aeae44a6cc4cd7" },
                { "zh-TW", "85f659654a77bab1af85e5ff8d22c5f79527d8aff580d44b885cccc83a361350b8be8cbe9f7dfe84a560d47bce634842f88ca00969994768912d3140c4473275" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.9.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "4bc2b9e1c3f2829bf9a0d7057b5d53e16a59aa1d575b6ed2e88b473decbea0cdc517998f929bfc3d15487a76fd11a9d62b7895fd6a8b0afedabb1d9f4ca21eaf" },
                { "ar", "9206e94617ca429e524fe41210165d36b416ffdcc184ab65b7d47a0f713063938cbc2719ab423c99da846e6874d39c842f945856a05e3067f456f05d3b14fe5d" },
                { "ast", "affadbeb0d9ccd6baa523c5d6c8a4a00aa36aab94cbec98b4a96035691b264ab7123509bb643b8281262aa064cf58032af0275abc717d404ad890c8bb9c2ef0c" },
                { "be", "5d1725ccdcbebd91d7ca6c99f8f571a88870375e14b5755d172b4371787602f7c0a191d9634e42010857d28eabde6cd50a2c8780ed253b963422e7046b99d263" },
                { "bg", "389b2d88361c72ea528ea331fc9969cb49bb1a85d3b2320d54ffa9e52eeb77df333aa6e7a273d3735f3881c42efc4bbe17b965e0f4d84e55aea1dd8f5ac27599" },
                { "br", "51afd7a2113260aa6423eae7191ea72a30b5717c2a637cbe3dc12a459e01a7a007082972c9581b982f37c7e08e4133c0ffe9977d99dd1002049717c280cd3b6f" },
                { "ca", "4d3e9321e5ea75340439ece8436827b635c3ea2e287c64824ffcb59bad5e6dcd3355262af05b13fadec8fc98e85fd1330747fa3724cf55a66a652d8e63186968" },
                { "cak", "41d740b75582061864e9ce575c0218112e5d1630217c504fb7771fa763b41699019c7d380ed4d6389037ab561b59dec4bc4a9b063a34577535f8a994b0af3726" },
                { "cs", "74245a688786c9900c52fb265e83646d780dc6eecc5e54d5e086ef3205e7a49d509cb598edd74b372b62d3884c121ac7d13405c06acfdb99bd59251dd75b8f08" },
                { "cy", "4fb76ba95bebfe6836c122492bc33f9324da92b077a897fad1f90ade46d8e969646c5bb8d686ca0ceeb3f7e07d56e8305207edcc0a0b09368c78413269a458bc" },
                { "da", "ac22d75ff59e99f08cdee898c335a117a27e6607d6f609571679b5fe4f3fc85c78e6f08688990d6a3c471fc2f433312f06605d264d7b0f60e4fb752e4a5b0ae6" },
                { "de", "88489817f5be815d6f30277549cfb5efe9daae4d5efdf5939444845e96b99359a78f5716823eca570ff7ea9d9cdf3f67b4eaa9c9781fbe1b4890c3b15dfcb174" },
                { "dsb", "a971d4e4680b07b610d19566324eb8e8b7026833dfed2a65b7ee87d916754eee15d21c45579103c9e5dd7a4f2415ada685a47029034d4b7c6828083da6a3a541" },
                { "el", "b4b3eb996ac2927423e0b3ab7c6bbdfabd3d23a51bf5c621ccc5ee59843483d7f23e2e2e0ec1a5aac411bc804c8eaf36e555fe12c4c862ea7ec18b7340a01e34" },
                { "en-CA", "a9ae112f66deecfc1d89730e154e0a95911002f9ae7be55d6e56aaeb2995183eed0219636b95f21832d3f7b65930db341ef8be335a44bfe1a530a9da36024be4" },
                { "en-GB", "6897fa1ec65ee02d4ed0e435701ab635ffe329cf9b8f63b5ff2659357a5a2362d07904089ca4798bafd1df2ad255a331327c1a88354368440faf12c39629e347" },
                { "en-US", "494e87c6d5f1715ce1ac671bca7dca28ff0a32cbff534df2d44154ee7fe245290e075d803f6f629777ed538cca6aedc585e1b2eeecd2357ce39fb80bbdc208ef" },
                { "es-AR", "7f0cb3ab6c3e1734f7b38a746dcf7b7b374e3bc008e2131debdd0824cfc5fa31f8b95e08f237290dee23f5e6a77558428816b56a2225d52b76cafc71273dfefb" },
                { "es-ES", "7592013a0558d1ef77d892f6dae5b67a8b7bc9f2122669ed375bedb27835f6b78d97d1d29c79fefa4edbc97ea76fb42497b0ee3cde63a0b0bd82886df0106f32" },
                { "es-MX", "468b753e0486c81cef1b0013c2613ef7f3664fb4c9c2f877eb1755c170ba4bb201198905a0ac6af3d7ca6ab272334aff441ea11592206b97812147963d060663" },
                { "et", "ff3f39ff7aaa6f7657e1a6dcc198bef0da092399d0ad168d5c16d49192454ff63a348e674ecbedaec60f2acd4e43d6564f99009891c0aec0a946bdc8391d7169" },
                { "eu", "f33aa5a76a9b0233f017e08e00bdf0ec636396054c38f48b08065f8ac69419d70581ec56b2960c6efb0dd50303f208325e27919e984f80a0db67d5abe11259cc" },
                { "fi", "5267326b17083e8f5338944d80d55a0715020543e38bbd0ee08cea754ecb41acb5e18598bb395fd4c0d1e2bba1044a2c981b0de838baa96f7c45bfbb815eb39a" },
                { "fr", "82f244fd51642e4511c34e35e6e20e6d5eecd6db7fa61d1031253b97c923f321d8a5422cd36b82d349e23ed39e8371dc805089d71417a946eb495ad772c7a79a" },
                { "fy-NL", "d1f860f605be8fa1c9237feb15c9cfc9feb9a7b149a0ff379a768bd4a6dd302f66ec82227352c2d8f6ed6e2b2ef76f1bea251d82a5b289194fffe0f56d1a80e0" },
                { "ga-IE", "4ea197517bd8a05ee7963ccf89c50a0e7fa36b1cd0695293951a8d5433aabd3c9e363c574f863816246558af1348d6f6f3bdf9bb66bfb367824db679c4a60f40" },
                { "gd", "d7c5b0a11140c2b76f83e37d7ee04ae28b75f00e6a4508bcea6ef39440ddcaa421cf9870087ceb344d13297bbb3f49df44407d1be292c98a27efb494b454f987" },
                { "gl", "a4a4a03cdec56b100ebe328e2213124ab9191d4ac7c01c91167b85f114c37822d77f5794a526b55179fa91c8a00d95145032a0ad8f4ce5303e657e952887f5c2" },
                { "he", "ef66a33c9bb97e05f52770532e0487e34d527727f30f099f665054b7235ff5f5182f4fdcfd294fb9085f578ca8dd26be0202fbbc4cb697ae8c39efa4f715c524" },
                { "hr", "8154643d1a90352a31f70481d318236f729bf11a1c418fe4f71a5c92a092fa35aeb2c3dbaf480aa4306080fe010e8ed5c7d6c37bf9432b9350f251d03a5be3c9" },
                { "hsb", "1ce7489fd9f73d9341cb25f6aeb2741c79133a85243e980b7faf1cbed714fcce9b55b42179529407e0fa2c356cbaa6c881362ee9c065953823f4b940a39240f5" },
                { "hu", "66e85f0fe20a37f5aa2a2152dae1bbe5e0c316b0ee1ad9e9a96b0420bb7a37bde8d76cba0c769c9d1d25dac813a293f932638b728840656a76870d7a917443e0" },
                { "hy-AM", "82b4d5d927b91b268abe69f5ffb0066a289dea9ae932063faa8861ff52f8da420cff7ffadc216a54bace667eda65f9dc7e4332f2b4dd9849e5c11b8f4826437c" },
                { "id", "d8ce3586b680ff08805974c30a196662a4635917f2d678efdee9ea55590628a3087d39706f3ba5f6a907066890c16ff36b5901aec90203aebbb3da975951f7de" },
                { "is", "1b307bf2de89b06c834233f864acceebe4fbaaabe07eff32da540a372498c234eb6359419713bc5624267adecd362cf8ef596f9bc9c27e5a5cb37821240e46ae" },
                { "it", "17bfa7b1f9c6581774af595d22e70c83f2e43df6285efa9eb94c9dd5f9d9c79c770ab633073759fd858eb168a8bd288d41ed1408863e9728046787d7c46cadf7" },
                { "ja", "fb0d8560080ed8e1f5b6f21d67e39e8d256663e54bc531a672b8ef1333b1425ff94cb15dd0604febc196b6848719787796eef68bf55f928f2a6362a9b2525a1a" },
                { "ka", "07d5b0b54f727aa29a2c6294c2b11b90b2d8ba078426a3f808ee2eccb8310f1d5571a61a8d894d6651828690bc24f63fd115fa46032d5c134cccf7bdd53f2ed1" },
                { "kab", "fc467b31ce9b3139635ed9ce608deec0c57fa4a6bdb2bdf5063cbf66dbca068cf397ee5e66327eb7f185e1462fe4b51735d2a1efd7adef0c2362574a0a11a2b4" },
                { "kk", "aefbfa8f217d9b9ece93472f0e40847c24c36aaf6f78ecdc9d948123fc690508c0edfe30c177ce6e64f39f91d8731ea5eed948d60597e04848de5d9961c8152e" },
                { "ko", "b52a45f6102ecf96f7a417d92428ba9c9009dfb64edb2d1c2d58185b4bcc51379198310a494975b9136cb0a1d24e4b1bc4231f83af191694b20aa3abf456b8f1" },
                { "lt", "56e9a8859f72b1ce229ee169885576ba23c62577ad6c9ae2408cade8ad04b5d40da03a38368c5b0c02cff56e85a429479af7cc3e1047d8c66b76a51d9bf687ea" },
                { "lv", "933a104df7fe7a62795e87cd577ac435e2750ebe500c9254cdaac4753d84df343d33638af19f81f91422f3a5a890087a03126aaf73c3626d0a2eca1143fa10ae" },
                { "ms", "3f10416e60d0ec073420bef5a1b6424537a6316b6ea3b569878cc956d3502de50a3e0737b24ce0cc11c928937882b70c17f1aff49bd7f7fd24f4311f08476295" },
                { "nb-NO", "b34266459a21f37b97556d7d01855f56653d2aa1aca3b58fb3ce8155a20c0ef639793a6222d7f2a4852c01aafee106af490c404250c712685948ee19e0463c88" },
                { "nl", "0854142a550f64fc1945bd93ea555b0868cabcc0b32f5d621244af085a3ec585d39d6a645e4818cb3ce28152a0e16a2ca5566c667f4bb4117495deab2911f71a" },
                { "nn-NO", "41238c96608bdd42b0b8bce5fbb6d5ec327d0219999cadf58faa559893e56e935b316ab72e2d6b413165ec5ee70603204a0b99ce04d682e5e0d7155154a69f09" },
                { "pa-IN", "bf1f874e1e5759393356b9fc803369fdfeb481b55b5da58127be52cce063676602b10f651e18fc8e4550962129ebd0a882ea54e74ea20840423d5ad1e02b9014" },
                { "pl", "f24bf2e69d6518709ac13b3032b76f9c4dd191b860d31f822e08e3e2ba6028ca25b99f5287d3cd4f549d5c523bd25c7db611ada969bd2e0c4426e5bb3cf5ea94" },
                { "pt-BR", "bc4f66ee02c97a59ddb708aca6079ae7a61f4112aa09f34c7ed4e81b176e271a0abdcd306ad7cee7f28a0731e709b83b85268ba0e0dd908a52f2c3f4a41c8bfe" },
                { "pt-PT", "e3258a30343c839dd7d19ed7b67e83f2086d8de310dd1d098ebec65a1dd8dd22b474c7c643902d0ff401ae23aea8f9eb2451bdf47f62f17892fd7b9c679c811b" },
                { "rm", "0f31b988945b36bfe9b67b9fd2ae007dcca7369e377144075e0d58e49067ad83c6f582413995aec9fbe08f0947d340f1bb26368a2bbc25d1e2247c412d81eae3" },
                { "ro", "6d457f3f0d0586e86a22fdad0fa47d1a53c2e0f4dda093c04ff06213d6b9c8358fdaafac67ad84808393d3155da82277662f0c71beb91504e87f276cfe1edd79" },
                { "ru", "4096700cbfa9ea23d47b538575aa19d57cfae20b0de61954a0651a657270ac889af6beb6daf6e18f7f4b7d8c0221ff97a87810f257c9db4f4d937f54bbca6674" },
                { "sk", "5dd9604f82d581fe4caef9b552ac7d30c32d522368b625b07e7067d1c5870defde321d030e6eb5bbff20fe44fdd36e57dcda63abe0ecf8e2bf8f6a3f9290ac68" },
                { "sl", "71703449422421a4255f6a3e9a30da7b4930432dc5a416a17bd66d25c57e65fec0b22bca07746d7ec74de4222bf8f7b55b9ca088f367b1c47ca5aeb88a951ca4" },
                { "sq", "84c86bd0ce3336af60992cdc28a3ff389eec2f456df884d0aaf2c619c65c2a87d75ff82944c805eb60dc88cc295d5bad584687405623b7d3c803f0f90650b19e" },
                { "sr", "3f195174ee76b17edd1a9c9535502664cd735dcd85e1c6c04703cbd3c9e7f1b5b606aed3164d4001d1857f4922cec7259b024eaafdebee548d47be8f941a58bb" },
                { "sv-SE", "7ce8b1754ed87b99c2cf829976c25d342fa9d59cfd25da477d87d62dc4eeac9e2fdf88623680fa259c7ed98a59184f5a91d9e63298d833d8a83c8b9ed2b5c1a5" },
                { "th", "fabe385b67a0a9c37d0d212a9b61f774898a5d9ca71bd03d140e0100b7e572c158e812e14a2824640ca6d9a22ad28cf9e25c633182722af561ac9abbdb49dbce" },
                { "tr", "ba8cb9221325bb9e178c5eccdf324aee401c4de0205ca81724121e3d2ea460c502be628f95785c299932c2cc4dc593a204a96dbdf5c5b469935ec62d04ce528b" },
                { "uk", "8883b905ad24abd429a882ec37454f819bbca11fec4d0c2e5983f13078079278ccfa4ce64392990dbe306ba0a63d7f8da3b2dad5d032473a6dbee4dee21329cd" },
                { "uz", "1631a9b2cc66a18337715df0b2a11d6b772502d2c6cd1f65ce62b3e2d5a31465b2f85f69f997825522d5fa235d68b77799a5f902da19eb7bdf2745e847498bc7" },
                { "vi", "bdd3a57af228ac6b4d8be19e2479a1fdee3bc15ee55b8abaf1ddb3a593c233451635b8c1159247034b8302d0d20b461cf1ca1fae6c73f06a5f37bb4528e83ace" },
                { "zh-CN", "cc2e40def30f9f726c2e75a48be4aa2871c659df070c809d3783ce438821e1dd81fdded828de907bc38fcb55bee6fac7d1bbcde6d20519e8ee3e771a930772b4" },
                { "zh-TW", "d9accd40d6d470766cf247074b67ae92c7240a7e1ab750ae94fc742b85a4a28f328f604cf915c52e068bcb76a7314b73abb3c7f92ef270bcfe89b1dcc4d9b65f" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "102.9.1";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                task = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
