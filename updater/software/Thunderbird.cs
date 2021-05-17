﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

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
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/thunderbird/releases/78.10.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "42f71ff6674c5a28c9ab3120013fb58f73e169074a47e5a04b9ad08e32812204ef3e1381be8b65876314057e1b856e4f665224c51090ef8dd1b33184da8f5987" },
                { "ar", "912bc74a1c64b72f92e3ade5ab129ced69b15cfbea239ac7412c7145a27874b0d0f92c7b975b7bec3f038df66687d8c18ba70593bcb2a69d6f186e7efe2ce309" },
                { "ast", "3f77c4f5cb0d2769c189d939fe4b852ddef7ce0fbb6f75d747d516e1ce6dcb6a5a8b9c1aff48ac759df67754dbaa27c56d7f7640b3ab5ad374d96a9fba079d7f" },
                { "be", "2a4f3dc0ab4f55afdd13585e699b7191644dca1f5f09d55bca125cba87649acbd70d10b7455a0a8fa571f8d1f71915492a18aa73f4d6fafdfba372338f9d7b8f" },
                { "bg", "2629b2d171838770480c7489e75fc6036d23beca6bff956222fa995d5eeb3540fea5b2aa6bdeb3427c6ce6d9508a96180ee2936bceaf4c98ef7d651c72b4d0c7" },
                { "br", "33dc5654532f6e52b2c31b33fd35774ffa9b23a4b110ec3c37da7b62d604de4284385d86e778d0f824b3ad0498631b3b7f5a80c50b21ee6e62b14f5c758ed8e6" },
                { "ca", "4b96361a02458c8e7c6fc84a6f4c6b4d2787497ed74eeb647430cb906fe55b9cdc893b02192c72a4ba5d11d113e5332e63d653084895aceb53432363a7c5fc32" },
                { "cak", "32eaefbe20c9513894345e24667e60c5b7676db035ad9a2ab1bc922c04bb8a6e6768e7dae608d078798eba8ef2281df6164d62a0db501ab9fd4900bf042031b1" },
                { "cs", "8529093cffb7ca65d33425b3bd04c08a913d6f1217d33887b990371c27ec6e5ebbd525cd043416b419d176177319dfa0b04278ac1ca3a9dad6f354c19ecb2b9b" },
                { "cy", "335a9ed1ff48909e5c8a4f4fcca29478994af95b672c23413ff85ebe914501c345c1e1fde96f910f68bfcc490e13d78f13c609317ebc1323a07d53100297e0aa" },
                { "da", "881c90ef6033cb2f7337993289a210f8ee6780afb4f500945042102a0ef6b2627c4ea450e88ea51a8731c4fb314f25fcad529e56c9725a06d731052a9c3af67b" },
                { "de", "941e85d98a0d19331dc7d71b889f099e97ee954c7735b6193d66a68f6738f482631d3f6d773d4465eed9644d9d3887906a9fcec5846bde1f479ac55ac0b4195a" },
                { "dsb", "8c3a70e3ca5cb685d121b3cc9736f2ee9eab368b222d483c784eafdaa92a31de9277ff84f1ad2e29e0f39cb75de1f83eaff968ede26c9ac332c74df2db5a46f3" },
                { "el", "b36d5416f54216860af02f4f57e110702543d2da6a32e8237cd09d2f71daac0827b772a73973fb06ba91e273097e24f9daa34e764f6060c54ab3eb839f52a8f6" },
                { "en-CA", "c62b8dc9222b7158cda0bd2a975ad828c871d8b2a3c61b09f793f415b6becdb2d78ba123696b97a63931216fa8805f4fc5923591ed7edfdf0ccef74d695f388e" },
                { "en-GB", "05817ab4d8da1209300f76ab01ecfb120b2f36506d90cecbe15d175d3b7e5d8c5cae804e0b3917e64e1d18b6d1a9a22d0068c002aeb9cfec55669368a36fe3ed" },
                { "en-US", "d1b2705749f9533167791c51ff577482afd1c468dcfaa2d2a5242b2ff29c064d8ee82fa301ae7f257188a9e7bd6f77c4099cdba3f7a5a18efd436c79252a5a93" },
                { "es-AR", "0917bed46cdb395d4052116db52b567ab5d115686ea6fe3d92c0935e4aefb35ed7420ed7c4bad997457be102b1bc07c12744c85d82f2816141824ce768b5af61" },
                { "es-ES", "5d8054ec7a5787fbe2bdb93cf3a5f6ead41b5f36e50cc1b9143095693777b4a2b93e6242c7813feaf69e8631b2d9917d976d934e5c80c92e278d27bbaafece4b" },
                { "et", "618b0b13727c7e45d74de1d0867614acf9458c04ce46a062349202d505a1711738c413a008a433009702e52d3cba229eb544ae8f75fc64916756b6409acc7809" },
                { "eu", "53dc77f2314744a88a347318ad384d9f37a7bdf3b8aedaa4aea4e2ac2e329dc79b3fb70a825dae8215a94bcd76ad108768acce49ccaf52a37b8b609d2f47a255" },
                { "fa", "ef9dd9f0745d83233a48af94ddc2d99e205320232a7d740ee1092943c832ac4adafaa8b74cb1476982a42517c597a4ae1782af8c0da8ffea75396baa94c788a9" },
                { "fi", "f4d3d7533505994c25c7dba659c6f8205e4067857c9b59dd85a3b09de3ba3cb7e4f249bd5480653721802f56a0959f5c615a639c24fa7619d548d1dacc392269" },
                { "fr", "0954a1f5f48b8ae9a070e7137062766aca6b7aea1e7633a612b732a219c120833aa14d4d4ee5066d629c10c6d2ae0882939558f5e440bf47ff6cf13ba8791b6a" },
                { "fy-NL", "78b6fce8bbf4b97276eb1579e5b770023bc082fb5e98571f0f918ec7649db53ee9926b67f03f92cbf37c2aaeb2038c48532d8372b63952b7536d27f9d6dced4a" },
                { "ga-IE", "c43996738207721b9cb8c8a69a763ef0ac2558763386fcf83146cf8b7b08375d1324b4aa20d07c6cffe08c4a7938587ea3483ad5310aaa44229fca89e926aec7" },
                { "gd", "256a852af3c066f35178be817405c38fea60c48c01422fda204365edab35b3cc4e3d7ed5f4f2b0d031f58d2658e0828de0ea799ddfba56b2035985b8887e1710" },
                { "gl", "419a963c1e8563ad49dd5bb4cb10ffcb8cce5db43ff37c133990db5069c4b36b7689e9efc49123a97097ccb10f19b01d1509c072b18173d1e5e4be9957eba0bf" },
                { "he", "4824a1f2acdf5a2803d46c33f2eb037192eddb7c81817d33c1b410825b8450b4383eeee0e0775a1d4386736eabc8a7c1eca27b097d198b12f98a92af14ae8b00" },
                { "hr", "702131ebec57e84b7d6f143fc40bae897c282f7dfd6f5681e44cbca9808017c9adfb0031f47836fe4f3655563e251289f508b4a8524c29e80d9902fd0983ffbd" },
                { "hsb", "c4d0007b4fc34aeb950f89366d138a818abe72a50c1f4f2988b41fecdd415b71b2e79a081749bedecda8cc2c3befb452024168c3fa6da3177c42874f24c1ec5b" },
                { "hu", "6f35727f105f1930dd171e690ba191d9fde69fa90b7a25c5fecad3421518536bfd65ca5dfe31d1813fb7ec7f23e39ea56786ad443fac7cc59d08698760e6b87b" },
                { "hy-AM", "1d95cd507bf01ccffe28ef03685348a2714a9d2c6bbf7c8ebac72c7bcc3e0eb32f39cb09d5f255f71dbb71fabd74ab81b2e0e3e142fdf463649a81eb85ac0b3a" },
                { "id", "0d2c58899320a33da7340a3cf87a5072ba6837e026efcd5a6fa8127b0bb465510e08dbba0e9cb3d62788af8cf3067b5f2238b22f01d75f5e4c73e28b07fbfa3f" },
                { "is", "28030bd1895eea4f49f59aabec9e05545311663715f877d66b42a902c41480af25b874290bfd4f92150af9eee4574395c32bfdd242d1d1ee85e7f3f181150c55" },
                { "it", "c22730bb046316e4353a2b8a9d36793e5409b97654fcd100409f6e75c63d7f72ade561ebc781031b4167a2e6740326506d93721ee1d709c3982016c29a6188d5" },
                { "ja", "1272b95bb2064450d895a1891483e8cf7014572ac2bca64d3717773a3e026eab0d72bfd49aeab267a2f2fca1db5790dafc834016d99620cc35ddf231cf603864" },
                { "ka", "d53b7e0a519fb73d145884a3e6fd0f4e6857329e4abbac262a317926e67ac5ff53c5575c438a700e51e458c26578211c7559a0adaf8aa6eeae1a5b00a23192ef" },
                { "kab", "c5f29fa0f0d495413131167ae478758e572791ad2596bb82267c787a6203213d071970b75c573bebf0be3cb177c5270711c2c16a41e7a004cb9dd63efe25b2b8" },
                { "kk", "86b8abca0d1d42f247d62511be7c86f013ed8287738789a8cd81d8a02026c0e9fa974612462e9eda7bfb222d5a565e693c894958bc27cb494fcde79e174cba74" },
                { "ko", "276d5f48716536eee1db9be79e18726f544648734f4e22f37ed42bbceb6ebc890eb4b9b363542aadfd010bcbc7f6f8610b73b348c8d0061803e0d3a51deff824" },
                { "lt", "2665f47fccfb3f03e7ecadc34c59db7eaab17f65de640c16de45a8456202594b17b3a772630aa0d87e8659d67e729cfd8fc64698bcfdcdfaba22e0d6160477de" },
                { "ms", "67f1a03c87f3adb1068573e74a14c51eb165adf4dfb868ef1d5b55144f4505aac7841610ffc0b9331da7e4afcdfc9fd6167fe5ad12af26f276badd08251c8441" },
                { "nb-NO", "c15928dfa3da17fb0392eb590925e5968c4c4ba793099764e51ee0ff7f627f138826878a8e3794c3b5185ab84660fecf0638e54953afd9d43ede63302057ffd2" },
                { "nl", "e3b2fecb55ba53cf9358982330f2965c88308bfe04d6672fec3e22045eff6800c0ef39e2527e1b3af316574dff751e183e76dbd7707be11e701cab8ebbb3904c" },
                { "nn-NO", "6c9bcde94f17c09542c32803f54f648010df57430ad9bd22c471542359b7c71b318bbb4c24257dbb6480390e96d8803d7cc5d7b9267dd4ce1bfc75cda461f272" },
                { "pa-IN", "bd7e9d008b23ec2480e66639387cc12f676f78374fdb28cbb0ac530351b552d218186a82224e81497a2a4ef8fde161d74e9eb7da76f87df9dab59fac0c8f26db" },
                { "pl", "5bbd4938d4074fdb6e1d495f8048aaa040e9b8083df91baba479f5af8910eef6e1fd2d12f1f834c5f547e5c06ec3617cbcfebf8d7887e642568270f438faa4ac" },
                { "pt-BR", "1e93cc769c476693d2b3127c7e704d2622380b9249c8c5ebffef6caee4e9abdf81db1ccd80c3e4a047537223841b9ea6ff95ca39ecfa88b0cfc999d60751afdc" },
                { "pt-PT", "52faee4982e0370c314be33eadafc7b09691721829ca45b3fa47b7282088e2150e4e94594695bdab59d4a2c2d4f3cdafb6638426cfd3aea6accf2d46ddd84106" },
                { "rm", "0074069c361c34d26f89907b95d6d4a6165c78797f00091d4e15607fb7263a064a40c852ff6bc79ba9e8d0fbf6b239aed91703c3870d580a240cb2ea4d256ada" },
                { "ro", "8b027a0f8fde42608518cc7dbf59b13d98708965e84b2973230e35797e90a3cc8492bd243585784ccd9de9f3e9ca69ce62427e622deba0c88436b2f685398d23" },
                { "ru", "cf4ce86c1f4adb5538eb86f2921eb368e713e16206dd5217e45ca2aa526216b918780d1084f6740eee9abff2dffc35704288d9a186f36063d97af7112f24a969" },
                { "si", "12968af886fc19d17137d6e845a0bf6963a30cff40701ba41d7b1d4630a8ffb91e8915ae5682b8c0e0f6401c4d730ce293e93a628159ef91c113961ca3ceb837" },
                { "sk", "03b102cec0673fd9b9a55d2a3f2d28a1731bcf5a8ec16156b296d5178d3d43745a8d499f37f8d818f43c8b0594c9b029c6b7affca6f47eb0ea0c8f982795aedc" },
                { "sl", "5a7531248c08edfb5cd1d7ad355d469645658ed23a5486cdb43515ee8466dcc6e29dda39c6e584791350e85fe877baa8ab66bdde0363e71b8900577ebe1919b6" },
                { "sq", "9a1441ec6e6589d4e72c278ea92af0a5228e21b64ce9ca648d393fb0b2cca72382d1eb0d05c4f64c1a4b689571ceb83a84e76d82bd1c0a4c87c9757cab300a9c" },
                { "sr", "66286ab46e6642d9d89d020b98a5cf758aa000ada49d295f9a9c0611e43934c86593b7c79c42336e8c77fa94f1636a22bdefd9781d28730bd4b88d70586d2ee9" },
                { "sv-SE", "d8dfae309ba7776e124d2b55e9b2e5e9c771c97de1bf498d3e281b7fdf56a923440f9783983792de2fbd1a12932672c98bc5f1f9eb7b7860faa97b0448c26e15" },
                { "th", "e810a97f99551f8cb509d12daf756c951e81906a7d5f4ca9d785b55efce483f8d5225697d0daccaacde10ac20ea8451e870cf547b9143d791506c4061408da02" },
                { "tr", "b63427d6b89ba3df728a90e9c237475d53cb815aac84fef2f1446300260f62f8abd90b7958004fc753b92f7adeebcf183a9fd9db3dc7f89d9611eec6df878a48" },
                { "uk", "3d64e2d215835175bc2fc7a2d2b60f16200b47723022d220acf14380aae745c4c71b8e0d692304483817c9a2492cac13ef023054496545bb8eb0564ed01e326c" },
                { "uz", "05f321d78b5914b83693a52f75546a711719b63cc332141138eb3357ae664706f1aa15ed231cd9d42e3558a74a67541ef563f6acc4e3d6130e8b3048237f0ed7" },
                { "vi", "478b33208757c52836f4c3202fc1db6d7f0311f5b9d06377a2aabf6a8a22ea0d6802d79a2d1fd3cd5ed836aec07c3ba75e9a1e2e8ccfc69805f3a99cc0add3d0" },
                { "zh-CN", "636fc59c44fceb9bbcedb80dabf58e5cf814dec9bce74205dcc34f62ed4b2006ea4295cd29577b1ac9e47b523f9ea8291a6a5fdc9b62fe122e185e1182f92571" },
                { "zh-TW", "2d099e0f4636200e21eba37e2a5c425963bcdfbc1ce88dab01736704a58d880b8c1593ce0c6837c3ba8ab67cb0e469d9c1806f5a00ec7779e44f14d15898d48a" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/78.10.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "d51176423d5d3276e0725b1c1608cc5bb5e8a5ea51176e9cacd297a7ec78034b46694878405642968eaa9346feb4a616f7127ac7b36fb137985fed89bbf56478" },
                { "ar", "ac7f0df47a71ccab5caf167adbca45a10c40f3dc304eb451ed4ee07202248d5c2c0cc76b7deed88739a232e33a46d85882de0673870700cab7cb2049fe90a4ba" },
                { "ast", "54245af5966f594c43c07013fff6d6525030a3a90f069238f390883f254eb7bbadcf7db81d77a1792e8ade437ae3c11fca176cc543d57236488fa2530547b08c" },
                { "be", "91d10df284c0c47cf71834035ba035c0875abd88f738113e1157e109ce13d3cc4f981f1a96de8f627a0d9b36a0a8540dbfb534bd36ede2846dad1b421cbcee8f" },
                { "bg", "62325b737fce9f529c19555108d7e9d282ff9fa402121d46ffe82037dbb91f54b4bcc3eed183f3feb947bc073b4d63930f9c8c129d74a9b01f81c522adae6404" },
                { "br", "a5938bc10986ff87330901b0a68e5a33cadf0bcbfeb55b69f373efb9cb8b15204230deeecb6a47f4f945c9a765ffd9a6c2b501395e1c43eb54844618e3a316c1" },
                { "ca", "f62d3e1a95a9bdb54fead234f907707d2a1cb27372e702de2e2e6e5af8d05f1c74ac08373ca2f29cfa1bd320f3ff735db9b9344c55d58cd914d97a2fe46bdb92" },
                { "cak", "dbe2cf8b97917588f146661703d71097f9745f79942a564c8a73c4bbde9125245d0de8bdbd891c25a2724fdf3fbb1dc2f346046247b48397b7c058bd40fd7662" },
                { "cs", "81f3087cd52faee1455b57728add607982b16a3248932e7d879f7dd15b45de247fa3238e2562963747e2a606de58eb2355284a3276148c1048f63046e2167a44" },
                { "cy", "85cdff19fe14bc249456f6ba62f4d30f72d88137bcef464b552d8b6a7e689c3ee52a341b0f9a19e79b317aacb59342aa519a2e0e156ace8fe36811311d136eb4" },
                { "da", "7ad852d4a3efdfa4785a2d53e0bb6f2d2b0f1f711536c6d39e4be1754f822b3a9f1dcf202142d1cc476e0a412e38f7b53ed22164de1af9158d001f0dc812b7bd" },
                { "de", "8bed8b3bd0c757b1225df27124b383e70699b012e37233ffb8f4fe5473d8332ea81774636827f2bf1f25e40a83bfedbd0b11fdc1ab86e0b7363ad41307eefcdb" },
                { "dsb", "0aae0b7cdf6193063b09b8a298591cde275878bc91f7f49e49c6844c720d85f600b574950b147bc9d37da9be2b17d664693e7da290632a2243d4b5556038f552" },
                { "el", "554ed3fcd1b5c072395237160a91a6f1967e06400d656bcf6c3f36b028e64af8e482d11a8c4e062f0f88b6bd1d7a646391078b99787e9307f396e89a5bd8d0b5" },
                { "en-CA", "4a44b54435dfb178477f68d5ad67996fdfd6c2ca4fd2130c866cd1d9b9f9a4d515dae7c30c158c67de7151fd73d5d494383c14fa4a815a6fb51bc88035bed22d" },
                { "en-GB", "1ce723a285d0c826e29b90d8fa4008429154d109b510434d62902c3fab3c77ee5c7b853bb28118c4518cbf8e9c7efa9ea7d91f51878908e26f92d2de68d8b5ce" },
                { "en-US", "226e50fe6fc8a37fb3b77ee225fd1426c45b405ee8b93cb6fe12e518af75385499ba2b95ba7fe0dda0c5e2447f1e170dc814e0603d86db4982a723a94d33188b" },
                { "es-AR", "ce2c86dc9252626d76a806ae062b571a4a8a359e687d0d999587868213b5d4bc5dafddf2bd959173b928577f3aa5e9646b1557afe68e7acf7cf0f0707d86ae60" },
                { "es-ES", "4ba68c1bffb0041bd5e2d1836603e0521f81a8dcfa95b7ca04b8bf81ac04ee4233cb4dc0f7ca83cff6e600eb2e4b4ec7b7658318a0112d98294ef093f407d5d9" },
                { "et", "d946c7f28a6ab37686090412a780104212697f36b0c39439d9f396740b4f977de8b9acbfe6d991a5b2773bfc317e7a9fa2384e7d6dd3009ca183293f0a9460b6" },
                { "eu", "cb21a890c39e177391095997642d931b5035680b4cd3e8c1f3cddfb2a324946ea1d4cb0ed9991d42815b6e304d059f8851d5dfc970121c4c1f021b71e983fa30" },
                { "fa", "905e594558c4b3892037d9c447ad5f85c99448e4e805e97d09a856693aa7f5aeba575ee9bce0b5af906ec975c80fb4417306a59405a04b748840502d0ef7505e" },
                { "fi", "7b955ff82a176d24955898f1edb1fd83a09b9851d1f2dc5c735032df33075bb6c0551fcb40e7c337d84adf9e7881ccf29b97809954adb1d4acfc5a6caaebdfa2" },
                { "fr", "f1bdaa7de8629fe8b2bd63b88cee1f29f28a185159f1187c8526a0cbddd7cdf5d57c07c2e3f99988ac36597f2db588d06bdf6a1e7ee1d7e1fb9b4e9687aac484" },
                { "fy-NL", "43f4676be16aee9db19c613bff122ac1ced3a80e157ba08377f9028639ec06422ea20c4dd69885f774b9c84430c8f2c9d6f3ba70ee4e7128027b5cd281ec5279" },
                { "ga-IE", "9e04c8dd7e5d525a0536b26a667436674a00b215c62ed7ddccf1a850843c8cfd48c5b04cb22129f33892f44e3a53d3fa0b5f718f620b17a9fcbeeaa7d56a2157" },
                { "gd", "c5881b6f37b57d6bc48c9c8052868b4adf1a6a0e5507df5b5aa29d6dce31a880c7679520455f3ff695850b0c1609026e392102a9a76b10868f8494b5a7468d58" },
                { "gl", "ef7166a0bc51036bc4118f8d0ca1dfb0174b5f55b386a99c0c3c31cb1c26e1e0195a6c264a114c709c3935c6efc76e5ecf44a0b122f9239ae171df1ffab0e622" },
                { "he", "d6ad36e11f0039407e00c3ac0524804d9143c61402d2b203aafef84b6b9bca9d368d1b9a91912397d050a6e2fd894e197a1a271d11ca2bc5f5b0a9772fd83218" },
                { "hr", "82760d6e0212f9209e4b26e37453586af8285cb133e8c52599f2e3467b0a83ecc1da11bec74927b0e6aee01f15f4c411a2a6c78c0bd68062ff8f498b828268f4" },
                { "hsb", "0489baffad0ce515e0c9b43a85f7cf0f2315a25be6bda1e845668035744bdd343f37b98293d0760e5a8ab21d6c79f84ec331afa293e6ec6a6725fb680fe3af6f" },
                { "hu", "8ff4eded263d741fbe55800809af7d27bc5e79488422fa61b24f76362b6472265e88ec9ef912371fa9889554f894ceced2baac1327cfc41976ccab14d2f7898f" },
                { "hy-AM", "7c962869101ac01b01d4da4d4785ddc470eb9388e2fece77f996680abeb0f940920aa1b2aaf5f8ab8e5278b13d348fcb2b365c633ba3b7b115b413a4bddec5ea" },
                { "id", "b6e3e6b8ed3e172446f77eccf7ad7d8ffef750c43c446036757e76a522642799940f63f31967c7f63a62ed0385573340a4f9eee4268e2e5e56d99d046931717b" },
                { "is", "9131031f17231c3d2dfbd850b6f78711ff1178bbafc6ee861e986b51519aeed4440319ee7d633c18c214982d7bd3d69467a71736d6acfdde0f56e927b4883d1a" },
                { "it", "cead4914289343bba71ffb3cb685cbe91cb4aea60e3840df47c362192ad2a3a57c2a4e86a6223e64c6e494ac0d045c53e99732e13b0033e524c2ec8de1ae67e4" },
                { "ja", "5eed01c3df54048b22d64efc2df976d7b0992e3c902d0dedc9cb90dc5e99bf5858eee0a51a0dcb83081461e4ae2347bdb817c1119c85fd90f3dff5f105882093" },
                { "ka", "b1c8cae0b8ccc39c21ee7820eda1ab8113d0885db6dfb6119e46305a7c86f4621200e6a93d397e738235fd5dd51a81525f54ba3ba81100857725605f7b460128" },
                { "kab", "8b78d0539f9eb9bfe325c4246393ec5d5cadf7fc69613c100a44598446d87e2570107374ab641b7aff618362671319fa4f1a3c3e80f17c769ba1d58387f45b13" },
                { "kk", "623fb50810da153363d042c91efd19091f976a87c09e449e7157d40a7f4845dd44f0e00d2b5646930bbd3c226ee3fe6e33f5309d729c5f743a94fb3de206bb01" },
                { "ko", "df9ef925db77bf6f879ae8598352f28caf728e48debaec0d918e1ffadc3b4e3f7a5411fe09a40cbe785659f3dec441f3311c56d39a2b3ebefa8922df04bb7245" },
                { "lt", "b883f22e2a09f86d0ccad40af666f3fca0106e78e3564b1e2a2697aae8d8c7ce300e3f202c983a57900e4b1e66c5fb33ccac6f3c8bf3b2406923ce7233472b9e" },
                { "ms", "4c1b26d7f0914a76946d0cadd0b5912d45f1f4b06448bfda0d1b6c2d755ccb7396dd3aa3f40639df1cbf4b1cb79706ee9b94863122fa05e006ace68496e53821" },
                { "nb-NO", "468bfa2c34d7dbc22073a99068f34076b97d2910312c777c8d6e39512ceff7771f5a02d1fc0c853ef62849b60fcfdeff61be407055af19600d24a908c295c750" },
                { "nl", "fb5f5e89527bdcd300c792c0be0ef086da5fc51a3a976f852b4a6b5bc24fb74c19315ccdb2cacf06cc87cc57c2cfe0973b54981498246202650b062b5c30a172" },
                { "nn-NO", "08831658b562a5902d89a4d2209efbfe6a60240868f05f003815b6ba9137c827c89176433eb7417576c582b049dc176df9fb1b82aea738fcaa238a934fca5202" },
                { "pa-IN", "b9244dfd5d20a45fcd2a850348bcdb635ac7dbe3c848a52ac547c7828e231bf4e063243efcb2eeaf1c2e83cb845d0103729bef40aab071194097dc2505eb1a18" },
                { "pl", "77d1e1e469df311773519fd61a631ddf7ee6e59efa2522ce9ab780ed81355706adcc0f3b38d909d1b7b66cf5ae8f14b75f4d2b95703a857bb269ad05a9c84963" },
                { "pt-BR", "d5bf35f6806d5ffadb5e1a150945ff8e5b8ed423412e011b3457d6184419e532c8941d4a7389ceba517a3f4479790a263f396807b6d5abce2d153477f91aaebd" },
                { "pt-PT", "37603fbe05ed4fe8fb347a609409f2daf370ae89d2e755a5f1625e8b44e04279962a52373e869023a753758805c002f16b313181d766b3d1e5ad174100662125" },
                { "rm", "0a00757f2404ca3955000fa0e60c68d41495ff1e397cd10905d676081614ee2ae40c1591716f639a6a997fc017a4b2c2f0f02c2fda57e0669c6b7c05a133ec2e" },
                { "ro", "c12b0171353e505b621c0f84a08f9b4cea424a405e5ba320ac4cf314e3b1aa460637a5790795f2b5691991acdc5e276b21a10c907d7b981860f13664ff9985bb" },
                { "ru", "28c5552f57895af3c798e35da20a8a0842e3a06909cd661338af9ba2bf26fa8d28173706b9ff1bc273e518b2ddf25713ae49d571cad9dacbc9d412979a339662" },
                { "si", "e80b3e4d5f67acb9f8813f88d71034ab8bca071df955a8aa306a8a5b5376d434fa6093f218e71734b700f59d6f1f37b0fb34d4a2218d7c01cfe907c7189239dd" },
                { "sk", "21292a62c68116fe9bee01e1e4fa68342f2f9dee172bcdd0c6baaf811d4872bf6d52c1027f5c0260af9b42b6cdedd6333158ea0cc18a1987d1bc3c0bbc99842e" },
                { "sl", "de008922d5d3c1b713c6f7b9fb166b9e08e5b92381da185e75be908a079b8b48f5805110b38cc64287f8d412c06aca577ef4b897d0cf553d1383af897808aa41" },
                { "sq", "87b7ce0181a59c32b9f7dbb5cd3f5990f86089d5f1ac9496035e7e518082617db76e39e71ded967ebc70e9ad6e16c9f1422b4715852d4219d78faa8036844718" },
                { "sr", "aa7d59b407cd7ec56c9f90d8c74c992b509bd7aeacd1e589beba87e0d750cce263f6b7107fb804ccffcf07e6c08a9fc03262d691106dc94f78b5e533cbcc2270" },
                { "sv-SE", "cbcfb9a61063712ae8a3c4abc1adb98eac0735bdc2b9c4d2642d3aef3eba0d6717b527ebdbdbdc2007be9931f6b38a66ec04eba2efde55bd176437019e5f0fd3" },
                { "th", "2c1911776702b7023efe19891abfdab78aee5b7942c4c68a186214dc2d927869ade268496e0ba17c37bea2497818d3f11fb3d3094c35c48cd30c2f8dd42739fe" },
                { "tr", "f7e0672dd6f01d2a19fc96ca2d7587f8cf27821e6b7c6bb983e42bdb8ee1e2e0a9adc238b3b25387a670601fdebff539450dc195cd25156a6277764cef6dafaa" },
                { "uk", "d1543526588542a6efa40e14af49aaa1fef66880659a0cd0c59aca05d8cd06920c342c7dfc4a862ca79ab9684e2633ac3aa59c31d570bcd015f2fa28315218e6" },
                { "uz", "851e16622a1048b2144401b1f08a3c02cd0512267745eeffcbb1c9ecd842344ae3d02c9e5bcaa5e0b51bf275960216d665a88d1deac12efa3bb28775700f5d96" },
                { "vi", "7e4e004cb1a67dbd7eaa8a0ce4f20514239e53451c2f8dcc3367add6ce885f2c68358cc1af1da92693412f6bbac88bbb33b2bcfa322ff271f75b97e1478d9505" },
                { "zh-CN", "c4f9438ed3d083f73ee3aa9187a4744a09ff38c58812f0c8ff556b8952ac4b337b2b9743ca427cdb8ec6567c771da0e22be86f050148b10de5f0b460d2b5e9de" },
                { "zh-TW", "0952e574e09573ee8d108de17f5b1767e29b28006e7213d0a14d69a621507634b5c1dc8c9f24e330b85b3c3e7ef4adf57de68b3af91d1492dc30ea1ea78c4050" }
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
            const string version = "78.10.2";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird [0-9]+\\.[0-9]+(\\.[0-9]+)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird [0-9]+\\.[0-9]+(\\.[0-9]+)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
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
        /// <returns>Returns a string containing the checksum, if successfull.
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
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
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
            logger.Debug("Searching for newer version of Thunderbird (" + languageCode + ")...");
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
