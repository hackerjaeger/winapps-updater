﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024, 2025  Dirk Stolle

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
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
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
            if (!d32.TryGetValue(languageCode, out checksum32Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.TryGetValue(languageCode, out checksum64Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/135.0/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "684c4e31840cf9183f3661c66370509d6b79d221c6ebce39e6394d46a67538b363d9fde121365a56de86f97c0d48def88067ce24ec1b2af648890260e65ac157" },
                { "af", "41bba24e1bfe4879c71a204ec5ab52a8130e7d9b73d19d9844b358ee281b59d159a9cd8c1f34a670a9eea1d1e5fddd8ba838808b7c1983f312f80c59a0d04a4c" },
                { "an", "2348646782325dfd7138a96e9f9a2080708f04cb97511385a067003969241f370af14a540f5b4d4197d5e74bb2059a2a35cf29bbbc513d0ee9b27faf75744482" },
                { "ar", "0729efcb16c20bdeddd642606ff819f70868c1210ee0508f3d7cd337cc35cb6129c7e225d07b4a19b09eff0f9b6ba9190ff33172c5a17ccbef9b6765873f6180" },
                { "ast", "41c129c6325e07e55beee06ac11609fd6bb1e5636f7580081db7a3e4bb9a86b5a09d5cad5b66d6b02b6bb63c4f2e58e5304b1655aeb65a36c7e65cb4d25ed811" },
                { "az", "dbc5a6557b95abddb7f4cde3af43f8723614eca767b0be21ae7677fa324d7d17bc889118cc3ab4d7c09800914664506f4c67d60619939c8a35dc9749a0205c82" },
                { "be", "83ccc651950976d56db6b7f8e76de17d62a65f36549f351f2ba1a972c020f38356b5fae37000c201a6d0a918ce74d8d897a169a32d30474f73e68c1c9a6603db" },
                { "bg", "b739dfc46c7feb06a2b36cb8cc74fbf47ae890342be630486cf001951cfe92b7c294db87d1d92c548219fcde3b906cbb644d6084b23f3d51d7e8fb4276019485" },
                { "bn", "2c164f65d19d7ef40fea0b06dd7f4313dccf4a716786e55f00c25dc3e9042b89df3810bcc969be2083185daa66555f159aaede128fa80a072cbc7dde95860216" },
                { "br", "f6be380d5a8c9ef1b88dae97b5160bfbe9dbcf89a8ac6a03a983951ac58696669999f90de52208a3198111a1e132cfe3e24a7ecfa28acb6a5c629714821e0692" },
                { "bs", "d22819d6464594b2cc7ddf4fd149caad83a4d2e571dc27bddae2208dfaa6e4fb0c098435bf82996e5810d2f2a98e4da3a93cebbd8aaaf98d3a9f755690ae6c80" },
                { "ca", "2799fd649d2e01ed05e47155e666615c65e330e54d68d0adea17b2389668aa18bd6a06e8851294563b5f8e19fc51e73230d34b06a34911237f02fd68a3eaa8ec" },
                { "cak", "6b6dc74a6b56e4d0048ca469c46224e973da1bc2c8183077f628a5bba749b3e7460bc978b92bba032d0a8549b2e4fef217e59cc7bffb44d135fa1f8a4120b2d6" },
                { "cs", "6dabb7f366873c2805bca2dc5a09ef9c845406364b6c31e21d5628046f2887546f29c69e2cad66acf5e2b5c79eedb3982ece7515c976d08db72aa1d915008840" },
                { "cy", "4b8e06777db2351f39a14fc61c4958af767bc299383d4139b1500fd9d7cf45c29c297442e85f0a077ecd61f035576cfea11aac680089774060e07eb417541756" },
                { "da", "824fc339e272e94ce5fc7c23e1a9f8e2190bea0aece181b40f9e9ce837fc5bcb11ee758a1adee4f1a7f49cb1bba09a9336aab57c1b0d1b818171e40b404f9e1f" },
                { "de", "053a5e9392e8e20505d8cd83066d5c80d4bc189aab7e09536f80cec267fd9ab9bb72f8ea527da0d77517a47de56f52f2e1dfec64f9aad0d514bef6df75a76666" },
                { "dsb", "76861d4068db35a8ce603d55bacd822a04736e5dbacb249c6d6b01fa0f1afc02350e789e5c59d7ce5ebae0df86a76d62a6e131156b5a0bd599d98358f3d1f060" },
                { "el", "383c549c1354539131530c127378785a36c493a34f9932999a4b17af14de616950e2aa591fc99da690642c088559f9731065ecfa4157688e0a0155ac548a2bc5" },
                { "en-CA", "0a0296a8f4921484c12bb79a88208656996d3f877d85e9a74528523a03cd744b303c7932128ee24c1ad9b0ada54d46161ff11a2a9f7e68a0d767096f1949cc57" },
                { "en-GB", "d09092b62b5c5c9f62fb08f8594051b6065ade018d7fcc256f553da8fdfb97fd5dd7df9fc5329bd858c9b439ff888bd020ebf35f094147b87a07a51309a8b5a9" },
                { "en-US", "2ab8608bae11be475b5328298b89ca0d0c47bbdfad2561fcb4008d63ba79ba33e73cf7163f6be7e19f5ef0cd00c62d1d0e2eb50e8b3dee1ec0051b64225ad7fe" },
                { "eo", "0a9cf5a077018a6742534ed0c3608f3f28a95b71a695c62e57be1fcb62858f042d3493f06ffd1ae3d0753ea4663ee518892340db4628943d2c8622ca30bcaa53" },
                { "es-AR", "45926d01ca7e1ab851ea73221658cb86e9de3ad2d72aa5574950bccd0ee97c737373b9bc6b14a934b4bfd523ad9ea64899e02aaf85bedbfea59cf4889a306105" },
                { "es-CL", "7a2770194666efd38e68b0192817ed3b562cc28448c23c33d92893e425353efb04a15d029dc8045570e1257796a6cc9b3b2b5f4c236d30c58990545b0b70bbcf" },
                { "es-ES", "1d51808d69b71037afebff8f21b7f7e5b5bfc12b45df8db340898d05c976d2c71b06906c7d852da849124fdc16b5fb160f6a0a3c4295b96f179d56d9e40eda85" },
                { "es-MX", "3cd43d833178fe5ce5e3d3b51e158acd80b1bb67d1350a7d3acbed918c9995c4b55a31aadb5a05c52948722ebea90d79bdb9b7e3134e87fca35da689ac37f57a" },
                { "et", "db87408e3d5989005b39fce6c170d3ab6214a8ca59cb8173736fedbeca879fc57612596127cbea67d19450730808170deeb077325b3ca3367598a3788c4cbfe4" },
                { "eu", "def5193a3d8cc5e4f10b772d3284d9e41b6ffca2eb478eca70148521cb33f64e94a8e71e75053e0c6996fd91b5f024390ec4baeacba30c03b1ef218fa728c091" },
                { "fa", "196a6eaa50d014acf3719ce34b3a1aded6a70e1a52bf7646ba3a778837867d7592ef22b6985dd4b586bb48aeefb97a450c6b4cff1a5e42f65db194adba081d98" },
                { "ff", "31818d456e5bf96a2e072b27f919d465f8d93bb059ffd9a1755b093fa4c2764cc1063b900599254769d2e90618c6e94753476e04014a210dde509462cba0dad7" },
                { "fi", "d7db76bf8d040bfe4f5d3ca54f89d7454f5cd0098e902ff9119d4e7f04b2f1f8cac7e232bb09269ea44e15a3f16b78fa4761de288e1705ea02d3e5bf5be099e3" },
                { "fr", "0ab8fa512f5d0d382726559878dba618c5e64cf38181699a9f6d6e0ae3e76eb2614358535e421cdd9755d618606178bef464761d317753c0d4cf73cf3767b0ad" },
                { "fur", "67be6beedc210d87b5fc998b99209e379e0fc085d270ad33b9a1087b89abd057c837e4fde8dfca7b6a7c6b9486b0086d4814007e14b1d63ea5602dca1b4de300" },
                { "fy-NL", "f881cca0300021be075db01431baf92600a4ded09e7f9597a3e18040531789436fac930ff7bb8745a5ff9d6682e236c97329d04902f0a6230764a20f3c7ce4cd" },
                { "ga-IE", "f0b4c2a40c3b1a0c927807f72138f26881821122e934dab7c135b279cf055c079f00d29e8e82c4a311473a70aa383a4aa49392d4ed85cbed16a2f49f7a894543" },
                { "gd", "59555aa519e35f79c2a14ab736c1fe1e9fc3cddf6c0fa8a213aeef101fd34e55626f5e4a55bb949dcb5aaa84fd67b88e3f6843e148108ae8a8d480c003bec741" },
                { "gl", "af6db709294166c5a4cb48fbed4084c65508f008c3a96c4baeecbcdc9a9a5fd3a35fda4485ee4ebf923f0322c9e30e578675a33afb03f2e2d87bbb56000d32db" },
                { "gn", "fd1182838f1ca5211e5482e4584e006311a9469adcc4dbe82cbb998170df353f45acb8a89ffeb79e020899a85caa6c3c4d46ece3fb4b2b412b99e01987de6d36" },
                { "gu-IN", "2d370a5cbce015aa147efdcd1048a66d77fd0d04d97f482651c5546127d1f16f90899b524eb527cf0edc0c4d2cfddf29266ed9ccb3f294c62072f428e302224d" },
                { "he", "858465e1ed8e1aaf463d26f0948aac9e69b1d5b410e653b627f6fe7ba14e66e83136a96d42acd23fe34d2e2e5aa7396ca8c4dfa71c6edb6a19d5bb2f43ff953d" },
                { "hi-IN", "c37f5a551ffe7616553d726520d86ebeab5a0d7ad001856f6dbefd34cfc4cfab4af9fc3b5a5de8e210eaf5c6cef9eacd63b3103977c78553468bc7b6e22e3541" },
                { "hr", "2a290b7a1490df579390e0cf92e3eecfcf3e89a1a9545a33573ba420a6b7d213783128a1fbe669bb473a83b42cf171fba0a4b72d77883b365d0466f75b942a5d" },
                { "hsb", "6c5eb2709ab5bc8798878737dd08f21d06d115596066f5d3dc5c589e5473d6c9537f74458a8c26b10f48051d018b5eb10d47c26eb0e9422599ab47fa315b6d33" },
                { "hu", "2af837c1319005a00a09a0b47cdefa6be9f9f4d20fc4e66e409500ed6c7ddc52a793d07f2eb783d9caaf0c52a75eaaf6477937b3669796777b3ed4e8e8b2efd3" },
                { "hy-AM", "67f8176bf4ce377378b7551e4a323529b7ed8c0de388ed945034d77f772340b9ed594a8b32295872f74e06565952bd25c65037afc43198477a20c3ab7e095ad9" },
                { "ia", "f9ebe6df64adef3af21156bec9a860152f2b73bb4574d4dbfabe669fb9293ae4393b8a8b86876ba2e0326caa96fa68c3d12119193322b02df7ebf64a7c8fad7c" },
                { "id", "e22812ce57841ce71b2e51316a25f0dc96d5bf1d8ed6263de56a15d3c5b669986e7c5697c193c610aba93f3a5251c875d4419602ac9a8327af421f589cc5aa58" },
                { "is", "ae8254219fdd128541d1b10a704d644aad338289087e2c613b5da605e96609596e43b657f0e094b59398d723cd0dbdd0227703ac310a2f8dbc164563204878dd" },
                { "it", "3770e963ad8056b48965041aa62bab3c0cca44efd1148b4ad02de2f54be1e08d0e10a266b6d4c84702c31a98bd77d7fde661eb670e34b30b27783d42e7b68d6e" },
                { "ja", "cc35bde8d49f870098084adb7620f80bc7b6826d33d2992d9253de0932f9c5d4ba03ff35b293f1fa8dc27e2350e7afa267f54d437fb1d46d086e2db900263336" },
                { "ka", "41e55a3208bab12f312be37facdc6ffd85b3094c9d5b62d4210dcf38ec9d93a79670382ede820aa7fc6f5191e43ceb7a8de546f85ba9f2b0244bcb17d02af57c" },
                { "kab", "3003626fc4164222a80c316eb9a0895c1dfe91eb7befc3c1d4e23e2b3127157e1398419ffc5ae8d2fdb5777347f59aa41260ac92dc3cb523a6ba84876c03d650" },
                { "kk", "ccc848a2d60e71bd0ff52a5331b302fb505e5d6aa2fd1bdc63753454d6e692b5887f1172ead662fd661d47a0f7189846a3da434ff9a17a7f880522b9f29b74c5" },
                { "km", "abc6952cdc896a5adf09580de30b5289b565f868d58859cd0234c4d624b79502eed3c1b8fe04ac83fc185fb1dadcba77c3c7475c5ac44922c781c9abd3ffa419" },
                { "kn", "cb9bae3890afb13896d9aaab873f65d54f1babf24465893926b43573f8834a9d75af1800edddba4207da734bada9a49326781954397422b4eab0c1370e292b37" },
                { "ko", "87fddda868111e8e60176d5adebea27ad32af62847993af04d0ff6aa251c4a48e5174269b6dbca111a0d2b430fb2836a1cd260c6012a6720bfe45ca0d4e27e2c" },
                { "lij", "0d5d3c339f8ceed558a8bf2be0283fd7983ab6b96750c9acc9e04cfc44557370a8fa762d7aa196fb59950c2f096a473fb1c69d5699eca9c2392fa19a2e48f051" },
                { "lt", "a53dfd32f5e7cec7cb3a20c713f07797c3e9613434195ee1ad5540d1ff2f4968ca0329a133b0b3c42ddb60682f8284bcb750f37872c9a15e2c3b25158abd5ed5" },
                { "lv", "94722920bf0b5cf154a79d4183fee0aa10a37099735d32216c1ede1ca09fd51f286c95200b27e3551ab776ae70ef2135db711812f78ba9ac62ad3e5b3a484585" },
                { "mk", "c820d4451314984fc223014cad6160650ee4ca3424a04bffb35840706070f660c7259ea63d2c2213d102d6f54a75fe21747e3433cd011d22265693afc38117b6" },
                { "mr", "5f30081d5c7cbd0ac6ac30aa701370723a559253eedb9bebd3dd3594f1813db38ddf601fd742524ad24ea86ec6595e6a99b0a913e2fce0cabb0dc0659c8068da" },
                { "ms", "c2bebe09979fbe760322690d6b5c23fb31cee807665ce7896ac27b116f399d82a1f80a6db7dd814dd976e5df56ff2af60a6f218b19b348f91d92f020d6c0dc2a" },
                { "my", "dcfb9df0b7d90edf7633e126f5d4e5b0b2379866ca4144efb4d29f19982b0081e859d9b60d06a2fe52db5d1d46a3f7ab22c0fcfb2fc1b269839a1a9827f5226f" },
                { "nb-NO", "788e7b2b56dec01e241aaa6d3a71d57f5c79802a2e2c9078f462d7437e6c195c26612edd0132759d55f1358fa6ea269de8a8d3af6285148ae40a509a3e36e59e" },
                { "ne-NP", "2b1a0416513bfc303d8d74d7f16ca65446db9e3caabf4adcb8f0b33fce71d3d3dac0541b2f35676818a6e635979baf233aa7b28e8add9c3d2b5078090db8c3f2" },
                { "nl", "f32ab94893461d2b3472c685a53a48d0c8d76327616579a2d277b6decaba263ae514c11d18803c124250c505be54d0c6e6ff5ca33f44048c8d6e6aea055f7c23" },
                { "nn-NO", "cba5e30dfde6a5347afe168f176e1ba640b2db6947585465b0b8b51196288275bf46f9704cf10d09eab4efe1fb6b9bb907b4fa0cc3a4aef35cb246a1b0924d47" },
                { "oc", "9e5550391dc6bdf06c9e25ebec681333cb0df30b3c05cc0279fc926dfc209f21a43305f45c648d39a36b6fd87ee7bf4c00926fa89d3495613812274c135548bb" },
                { "pa-IN", "94aefc2f0e4d7aea34d38d314d2ac7170bf898a202ed90f65a1ccfae51f5a06fba5c315b538a5e3bf7be7421543ec80558189ba7e66578553d611b2ae5730253" },
                { "pl", "91fe4196615b67ba48e54c9b2d58264c362d23f6f20acbafbb6f6a1807f7cac202ba9f9248f889784701f96193258946637dffbd1e585a35c16d4a79ef8a6433" },
                { "pt-BR", "4b8ec1128eb6fdf830889a25e9517a05911bdd3a01337cbaed857235ad9008d65a4b727de6db13e113315242e14c2f1d7e27f983f2d216014d9e05390b01ed0e" },
                { "pt-PT", "fac57107cc8b779536ae5ea26e3f1d51d194fbd572af33ff3915bd67f0160a7892c192f18867430207fc138684a8c95516f8b6594f1d5174f948f2eba1c83f98" },
                { "rm", "d2c5ad80d470b8b5f2a6de20c83845d7e3a97bea35cc2279a26ff413ed000ce3cdfb3297d168e8cfa819a26df1b5dc73b748b60e1b467b8b2d7a7985ec574ead" },
                { "ro", "4c26451b7c08c0817646c018b039a3ac498b28c90a5430396f77cd6b0e44179011678200da818562050c0dec9bdb8a4956ec5b608a1727339321e4e8d4069d9e" },
                { "ru", "610c60374b78476d860dc44c1f26ef02526c4afbfc0a154d6225266d54e5b0a27da23cb193dbefce837c113b30afd99be3592f77576ff551073263db5a764089" },
                { "sat", "dec8cda1533f9b84c80c3d4d0abb4324dfb5d5a552fb7d3865e940bf94d9b0d6f0e1e0c1967e3718ed07d699e244c21b132281cf38e0f8b77d116be1bf32aeb2" },
                { "sc", "de4af4bb7463b263b0468be69c9681ebf6c3fa3638a62925c1f25569c8a79e82b797c4586366d60c283b12ff593b0280b2e7fce61287a4e098f440389e0a6ff5" },
                { "sco", "d14e23b3035d3aab7b75633900cd98cfb5f12abdceab90b13eba936bbd48d57f317c35d0cc968dbf8738e9198ef96af4ec47e39a1d62cb216f8bf038c6159879" },
                { "si", "823c0e865fb7a16d952edffb147c34039a5be80cffa6d721997a23a76cb41bfd09841469b63e7768c609a9977e499db77d4f4726dda15173cc901192836a7844" },
                { "sk", "4afee2c247652150dc4add6f6256932bd5dc83684bcb89e37c2c2c0f40569121d6346808a35c214c3e4a5c0eeb9326d2e4957b60c823d670ab606be494548431" },
                { "skr", "b24dc13816a75ed4989adebda7cc2ccdcc8226afac4f66465dece37aef1987b3caf835b943f1b47882d67bc9c8357943b49cbf0c95aac8271393f91a8b7e19c0" },
                { "sl", "998b78fcdfd64d5bdcc5af1f35b22652b6d14a916d20e669f3fec95400cf73aca69c651633becae148024729da97c4bcc6e76e2283a1053948c4f222975f209e" },
                { "son", "59b15d4df876d4017f54c8426d9583103d0de6669cd9ee987b93952f8b001d4bee128698d2726a6930ce825759115dc8859490ba7420726d6ffb42fc9452ac0f" },
                { "sq", "1fd000c5f5b0b462f62dd6164df9d79f145fe86f5397e90298962ad7a18860cedd973a3836f9322c2488957604f2919c4b6a7ed3fac3ee2d3fad2a06f96429f8" },
                { "sr", "c86941e4107e587d0ae440f090ed7c7c7a7cc3f26186341f383810245d05f2ff4df5528a7df4ef423a5dd82cd9f007c84f766497c528cc616e6b6c9784597edb" },
                { "sv-SE", "819433f2e83f5e8e3cb5eef525407d6456f11c9ef4b14be517f9cdb244647055c3782b5664bab459feaf8331209d02f9303980ae3306e6aa176e568ba6c6b996" },
                { "szl", "a47054d5a84f1a09ad447d45c7b2041c3d605ffee77f02f5a6b6629e380ff5b2c03628754f900a4e8a08d3d46bd254e4fa2a41c4bd61c93525b60ee465af11ca" },
                { "ta", "e1959932f4422fb05abe17f563307a334cefe69e186564cf1552053ead6247b1197218107d1dcb9c823ef84f93f5e0af63dfb71164bde0990046bbe48f17431d" },
                { "te", "8a479a8be0e462e837b6a77658848394e8c5930900d4ee41d5a432ba8f26634ac9091e0fa579490c524ec047cb37a81cf3797dcf2e745bfea21a025e82ffc260" },
                { "tg", "31905a191ddc5260c82bcc66932a0a51db03550acdb929f71ac0774ea69f41c938bf6057e329a1daba10ca5ab59ead4f7b80d30d8715c3b6b6528ed4537dbc17" },
                { "th", "95a9601c11ec1d0660e741dec3bd931f5fea139e07c3bb94d8c0e98a4143445107055d0a1e6a225fb829bf56dc6d75bc3f9b38a348a0320b7f0d404b876eab81" },
                { "tl", "e3ef2abd674ef7c1c531a53ee6584b8c9f7f82c6b6722d08aa77dad57b5c2276835f7c74e6c122abe571a4e2c7651f1bf74ab9286e5486b027bfc2eb46f935f6" },
                { "tr", "d97dd1c5274b1646ece51c00c1abed85d4e4fa4ba7d351fa2007dc9a6f89e1738395837edd5a77f1a35c8c0aae9e0afdef4d6131ff00800941fc626f3c67a93c" },
                { "trs", "2635c21ea6f2a49cf41994f89b3684b3155d4825bc5cb1e374b4145c097c22b47d3bd89972d4894bb4f2e57ab3c38b7c30bfec9b62219be2ed5e5d07dcb668e1" },
                { "uk", "c8bae9cb334e3a7a01c060d26de5675d19e7db630591ef6cec16a73093cd8fffec324aac85212fe698c7e2613277702d1c28011f76e3731960f1eec7f33821cf" },
                { "ur", "0e058442e9afb410392d07127e41f2bec503ad194d62959e4fb159f70e78fc57b3129d3978366fbf607a06739a6222949f92e1f709f3d562419cdf6b7733adf3" },
                { "uz", "122c761e61f3e62a8aae562218fb749c2a43b05f868ebd6110badee57622769982c97dd27b6d91b55af228d4754217e308df1ae30b7936a460b044014e3e459d" },
                { "vi", "241e8e509eaaeabd77c06c1ff91f369e0240687f1a349f6380cbed8b2b3d3b0d705b0ab61c21c9eb9e9f1797c3c6ae51c99d06b6e31a53da2210dc5bd883824b" },
                { "xh", "1a6836804e04e5a73c00b7e5d978e3be23c3cd8f1fbf698a74605e7953455a9a1ed8c4f1a2d9043fb3309f721f57e03fb6a79b08b33cf4e13cceef55febee07d" },
                { "zh-CN", "4fd3d5d59e39f4e0a79c4b4ee3d4e6a6268b38c3f3be8a1d329099412ea5577728f090feccbc879a0cc1008be2225137769124da211c94fd7807657c47b1bc8d" },
                { "zh-TW", "4e86b52bff6e7945ebc4fd57c5ad3ca8c01ad2ff367300f8f7e5a0db094b2982372797f0770a09e579bdc91fb2b4ea794526be519b3e89b47eb605aaddb10960" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/135.0/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "25222890f04b75da845898e8e1b2d86fa235832518912778e96e40c1707590ee435175d021fe09668d878981cb6875d15e7a3e34d0836f92f83ea376ed614ef9" },
                { "af", "004792ef9fc636b554ebaf6fd9b58eb3f78bf4a6a567b9d8b5995e29652637215295aab6b7c8fe4ad7e9c8913ed4d806820bbae372610e3182eef17519234215" },
                { "an", "0395f1285fea60ccf6b1b766c6f396d4829784457bf73fbca6843af0534b47d518c6d103c1659b7bd252b58ecdfe5f1da2d96d3e370d735556e2cda1c5f08714" },
                { "ar", "1a485c597bb0379f705f95428897c752fb78e5b08f298a0ee1f659b25cbae3fa31ea4111e95735f13a24e68426e111c89df31052a4aa8615ad846ef0c3a982ed" },
                { "ast", "9492bee4cf4789bc417d458dbda160e7e215c5648d20623916d22368870ff27d91dc17643ecedbbb9dbc6dd61717248a928944194289f7c1b9fb0a268ce670f5" },
                { "az", "3fc1d5899d9d0bbaaaf3c637b6422819b552e895095e589b566cd4b8a39f17b3fcef5368fe599f4cd0adaa0d1d0e294e870922692dbb139d295a0dfd83471ed1" },
                { "be", "efe6d6bd02e5a00495f3fd6584f92248762480f1be0591624ae0f6033e0e06bf304a47cd980bd3f594dfcaeccc5336fdb7aed72daf59f8d9f5cfe700e7935c07" },
                { "bg", "de8ad79a1e70ea532ba1960beb58ba5bbf798a7a25e952ef2c9d6713ea6e702c09ec3f39b0a75ec9d3244d3cb52e220075dcd97ff68e562bb9cf37dd80250376" },
                { "bn", "bfb8d8993875814ba56630e12fe915be307893cdcb12b6da7aee6c94423780cabf5f7bb51e17c300d204591d95cd3b86e12e82d849ebc3bb2b05e05429647f1f" },
                { "br", "991af30374508c55a2ba3bd4a9e4415ec59228c7bee10b59c969b09432e036c344bbe3f075c7631f88f97f733c8f5117fd326e1342bc1706a1e1fcc20fab65e6" },
                { "bs", "5df6c750b1a9325666d1c196efd7b49bbe21b84d0fa6b92ab474bd43c9da6a522f56e8de0185f281b75e0f28f4ae9fda2bdc4b5903e10f2c3260f22509e99f9a" },
                { "ca", "d65b089b0fb10f9b59f6b7f5205002118a9b78b955884a5eb3c86e8f2d5aedc16f54532bde7b34c5fc4e2482a9e5e1f1dfd14ce1ed03cf4e6c1303e98ec5bbd6" },
                { "cak", "57627ab05fada4457cd5096a1cd2b1e503fd23bfb9c71a61e9b269dbf3439c73092b44701f305a16282305d3766892597424f433214745070b5ab45ec23a9900" },
                { "cs", "693031227617a9b67c1e3d8b46540bb33666dd54ef44ce07d8060261c40c2e4a6f7c2007725ecb93c825ccb90b62df4ed2d12caff7df71d0f50016064d90f403" },
                { "cy", "ac6c1b9e6ecd4c5501beb631aad6ec1ee2be0ea43ee7d0ae0cf8ea5cba654e55edf63e1624a98aa8ae80fa9ee8fa6bf270f33d987bb6f2dd56d6f1cee724c204" },
                { "da", "64e59a55634e019c2860878e8c202eaf59578932d70fa9f2601433080171e0f037c9c1ddb272f181538da32b7d8e426d2b8460999f74cba56837853192746ace" },
                { "de", "90dc4176542d37d1b4d7fd7ddf0254907c10e9d68cf840dc8687ba2b87e3c2383052c31e4a144d9daefba1443bcaa8edc11b05460ff1f4a14be7894539cda527" },
                { "dsb", "dc524b0359c7af1432045aad36b1323441ffcd92f6522483ad508a0670eec4106d0c0e8290d5fea31bc8fb3841aeb7d3a71ba6e29490d786275ea2e1465f6772" },
                { "el", "391567bfdafa5344b36d3ed5c01a215b2fcd4dc0ef9da6d75595d67fc09d3003de5dcdcf7625490e42f888f38726dbe96fc8e295adf2c641ec102b87e407baab" },
                { "en-CA", "f8f35f77c467b30ac751e17eac96a948ba6d2c5139a3be29133309af5d5dd1989522acc7a84fc0ef58eddb906ceca00d9e174f28c64d161fea1e49d00cf8df0d" },
                { "en-GB", "4b626fb46c3f1b09182d77ccf1a135e0fba0914a57d65f1c70aee3e40fb8c6194d75d608ee89cb80e8a525f6a3eb441d39c2ac2ffee077ee47c3eaa80b7b5de2" },
                { "en-US", "4e69965ae59f793992f6579217e6bb2af194f41bcfb2325e4b15923a0cf3f5564228b926c73254831808f354a7eb03b62243f6b6ef291dfe2c8c10ed98caee40" },
                { "eo", "8649d4ece67e9970a077f4dc01fff2dc8b4040b30c00b1cc76bff1e6a59d126b467d619b6118cbcb8e8205d6d164c1f78055d5a0e8bbf028a5694ee7c7f0e52a" },
                { "es-AR", "dfb03bfb72d0cc2eb8e507c4b630203793a82ce784fc4c6d4857fe0462d42d0ae240229f4f20a114703e45857697e185840b429151f2e6b7b66e8b84ff0986e5" },
                { "es-CL", "e500b5c4957953665dd1be0cf89ffc9dfa052d2278c69cef04da18a3e16133661938b2b39ea1e95154ed1c502e55aa9bd519fc9427017b054ac7e3cee94c3a60" },
                { "es-ES", "58d889181b0c652002d5ebf7c86d465a269a19dcf8f434fee151f9693d36fc3901a58052a6682d95eabbb5e50d15e6892da4c1e86c9c86e4fc0a9b38720c3332" },
                { "es-MX", "f02311f7c5e9d8a5ead838722ea96a27778e2dc7a74cdc0db62de124a2303e857c2594bfbd1a9eb19685a19249706acf9fc3f01b3ab27fcc75a10f9095a14bd8" },
                { "et", "fc05352c6abc951bd99005631373802b3a35e222baef9fa3239db4934fbe17f83f2b707061c69a95e88f9c6ce1e5ca2b5fa5c030b71d1b5606b4a2e9bfcb1653" },
                { "eu", "f734f465d932ed1bf83df284b2ec4f5bfa9919ca7bed2ca5993981791f866bbd866aaf0b4102d858126161aef386126d4804ebcde95a6d2ba0dfccad9f130c70" },
                { "fa", "ebb7d9d23106b9c7b93378dc3b7ec0843eeda64f908ce0eadad541011b372487a2308bdc548a28efb0ff83b66519bb54205d6f40f1bc7cfd2d490399065f5a60" },
                { "ff", "fa2a4925f75ca77b1550ff2554f812006f8fdaac5ae0db4a497decba8474bd5feec7c97fac92d28c112e4fc6e992941f323888e9d843187fd942d057819d57a1" },
                { "fi", "068fea10f473c6429b34dd5db0a28643d21fc086d17b025db9a45f9e174d5e96ef6c39fe2ad2280564b5e73ca0c6cc4d488c8749209c5454dea71a0f1f87203a" },
                { "fr", "ee9e1ee9cd7f4098918181623960142f95f9073c03b8d22f01062a5d138805a19f7ee2f3ce8d79f32e43ff881b6c1291751829b6b08284e69500a95ba4cf5af7" },
                { "fur", "b7be72e40cb33ad274a45c95d4ad9afb2f7e87006338b46545750d22c3f30c95dccb8c59f2e0efc877c577e4e0aa3012097d5c83baef0152942df83b4fdd840a" },
                { "fy-NL", "bec40ecf5116ecfa70798e41573c96717c5283b3046a2c842437cfa81c1bd8b320256459f7da5d924cae9282ab0df15b8fe78317d13704757c08031e4d2b6cc2" },
                { "ga-IE", "db9ab9caa5267b393216c5dffe498632ff7b26ea06c48907bc02f04ba0b0ab9a8ee03649a6224efb50dc78fc918c7448ce7272e124dbe387b2df371784b4fda8" },
                { "gd", "1ee83d66edd25e3434399536f9cc086cf52cec3ba223f5d16d3309a65767e912758c316be1da1fd30d2f3ebb1a17ed514d6f15f7ca609d957f23522bc377b7f5" },
                { "gl", "ee8892c60c9b13fbd4170d664bea113eb73b07e679b674b3a7924460f87289028541e06dcfb5fbad0fcb7ba2d55405fb9d0d7941966dd78df81dbf8a1d705a3b" },
                { "gn", "cccd5904ac233038a8d7a584bc84831fdf03373bad9d21f1b4e4e3a22f2aa0d19c2b7dc61578a527024ac42739e083d01a921a7b0125b332d760d0990ef85196" },
                { "gu-IN", "2ac29bef2b1b47a77d7d21cb10a09d2181b61bd63d10eac0930a63e13ae5206bed3abc1e8075f345bc8fc1dbbe8e8a92c43e0ccbcacb23dd072eb11573a38ed8" },
                { "he", "d771b092e707fb0936f43b3ab9cd6dc6ff53836b7dfd66cc571d9c350e82cea5cd8febbf7f8f5936839528deb79512da0167a6e162e3061af6f0d2409e628dca" },
                { "hi-IN", "cb731cc0278e5f80cdf472869de51e6c97b2f72a0b0aa96b6307b87bf7f4d379789a33dbf14fe600a70447ab0eb07b696099111740176f5c93365914a74f018c" },
                { "hr", "8e4884c1c8ddf5738316c4f53c610b786b7b669eaa8bf8925924d1a5845a505787a34b50ffb27c76d28a490f1cd19d15ce5af8c648ef1dd54b0ed1cf28a513d6" },
                { "hsb", "85f2e6d90f0eb4c01a96018cbbf2f3b407950ce469433984a7792e6111ff9c907d1706169a7d5d52f4753686f209957053562e921e1ee4338c2cd08e90629199" },
                { "hu", "53f0db817e8562d038ef73d7a8c7e760980a22ecac3a833caee0840dcda20f558ad6aeae15134a47153c189c04a3aa8905c60bacd3f04096fb380bbc627a4bc7" },
                { "hy-AM", "813f8bbeb3e55574662e4e10775242cd753f8b33bbd0d655beefebb1b263a577dd8064915f01cdc62c55e1367553e896a14cb8465d26be1cb949930afe682add" },
                { "ia", "af623445a457c678582864aff86873c7a4ab7f04b3377d85505c2ef86b1fd00569c9db91dd3bc834cb2d288f4e3a47f1fd233ff0510570ef9a217da0ab8b4f27" },
                { "id", "5e11aa90c209b3042cae95318c83d730ac711e202d7a028e122a8fd188fad477eabfbbe2c9fb4a823865be86fee5b58f23ebbc28f241b8469e61e2b5d07768b8" },
                { "is", "08a45a2dd437f4ad672df6a5093d66de3bce9ff5e3cef5e6f25d7f0072119f7e715472b7fa3bbb70ad3561577fe4c4bf4f1606ee7f1624b697a81ad7a06c226d" },
                { "it", "a16878bc3bb8f36c6187396e154434f640cc2e19a26f96108df8edddba27e08f37c9c81c3c557f18d07e3e0d8a8ba64247d17123426c8d3440cac93a621e5f5c" },
                { "ja", "99b60bd69bc40d24dd82404ae4c548f80f8dfa261a769e9a61c1830363ff1c8c67a0ae65419d68e03f58731c2ae74f3bae3e259cd6fe1d6e8f1ffd2799176a57" },
                { "ka", "3d209c99819d74e917ac937d67d38b36f4027796390f06576f63401b3f4de32516687133ea691eaff3518e3d5f410f65a5edf72d28ef56c31b9e2e897811dd0e" },
                { "kab", "eb870187b99bbaaf7011f72f9f81e7f197b6312e3e21be9e464657a2ff5f14392a3ed3647479781de2f0380516f48d9ca6d4cd23e451fc7dd83be5f54e9f597b" },
                { "kk", "13679aee998345320f0c8f644fbec5d878cd3c7210d5d13689313507a9f4f26ec77ec2ac8b990f4f389592a98e179171077028650d220122f609aca22664cf07" },
                { "km", "4cfe91016183d92f7f0abd21cc8cfaf578c086020277c5f369f4f1c0592ee9323533359dfa9f37741a387287a6d600b244ded0255039b60bec45f68261ddfd1a" },
                { "kn", "47291c62f94d877305a9afcee22d90bbf942994c0bfb1bfea91c770cd47011ce2ec27c63fef3625956a1effeea493d72e29a315c197cbffbfb9cb1cb48aa1672" },
                { "ko", "6905edada307c3ae9686e493c54525664e635498c2d83611b96e12698e686822991a83b1ec9671b2deede726b82f5f7954276e22014c790d642c713b08c046c4" },
                { "lij", "cdf1bdd55dbcb37e271d0fbc62f355ff7d5bfd6a2ca4a9469613b262fdbdb47af0ce6d96c91832d5ea3885ef8cfe0aa806fb6502428c01ebe6a68e6322a8de39" },
                { "lt", "e918fdf81100e6aee5312e914401f43b0dab262a2104906297dc57381da9c5f4cc79f67f6dd01f61af83bcf76dd6fcc5a76ea0f4bd860978f43ce52961f2d37e" },
                { "lv", "2168b4fd8d810e01d84d7c7b3bdd8cfd006bf06412250c4eb46a85f77d52e48f040b0254ee6098a3ec76911d671642772e4ee290e6e790d19e395397986e69ff" },
                { "mk", "a1cd6b2ae4d57413b964bb8b65326609f2a8a9c9b6bc76ac205d2245ad46b1504732df31b1a94b100a330ff3ecacd8c300fd9a94e229c51502da8d58a8160cc4" },
                { "mr", "13d1f074e6084c232b96aa1b37632f8d103a83a9770528e362d9289084b89f8e5591081611c37830cd320dff866a44e39df9b6defe479a45adeacfe844880129" },
                { "ms", "a7aa6ab7f972e10d1cecc991f1c01f084140b65ccbafd9d81f2374c6e5c956dc772f729f04f52ff793c635dcd545a69cd48b03d7cd4e80e67fedc84ba2dda8e0" },
                { "my", "07ae6f7322153c7a1becab7e0edc620820395f1dd368d43146cf3d002e5806e88242a8429288b0a9d2d6b5538d859ff9dfb42ccc6b6de2898f2e78188a1d3741" },
                { "nb-NO", "fded86097fc56e30e0e70b6f3f28771232361a64afc505bc0f39aa224df10343be72f78e80deb068c5de944f1d3f855ee3a975d9d772042712ccaa63a28d0688" },
                { "ne-NP", "667aab0500917be015355554c65a470e0ba56a75c0bb33fb39d173d7db100d8c4cb02ea13484b1253c46878b7c1678fc7fadadaabbe96b8015e817d5736bd4a6" },
                { "nl", "b71a7b48949cfe16ead01a62589ff25af2a7e53dfd2a257c8c5a7d7aa90f9eb3924e27086400a07ba326ef67e0a6e013837956e6ee565a99a05e8d59ecbeb90f" },
                { "nn-NO", "56135d532944d9dd55d4f6162b09e9462f87329536b861ed84fc16b176d81bcbccd3d9acbcb86c6fdce5ede900861fa25f915cf596ecaf9b6e63ed7d75f6d94f" },
                { "oc", "f61f6063aa9bf43fd8bdf92e09e826a76190b9eeb3476dd4b5623f436e9d9a1682a8980d338fe67f16668b37ce0026d2b0f09106acea94e3013028542f6147b8" },
                { "pa-IN", "3b607a1946ad65d78586667b0382daaf13bb8db4d82770c3208bb539b3bfa41f9ca4661f84f1bcecf4a6bb476ae6ac1b872473619e0070bd1363bd3aaf7b2362" },
                { "pl", "b4485fb6c0e234c657f089c090abf1c9022e5a5f9709bb027eff107b452112cb25cc10534f6edf07a05a2b0acd3597c40d9f3917ad58f71ce85bc07331a3b2f9" },
                { "pt-BR", "6e3d8cd94a5caa49a8958b1026d8f961ed41d41982c0cc035615fe7db65d02bc5b971d010ae12513ef2be5560c72ae293bb00485ba1d96009db610b9902d5c6c" },
                { "pt-PT", "16c3d9f734bd4d77e6beb6073f8b92f599fa788664581e9b4c53c97f2e7a0d6f644bcb7bffa78765dd6f0d5ce76baf9ac8cc53fc2ca9b4fbbe2cc9bb1e74c353" },
                { "rm", "563ced2b9b35eaf6e8edf5521e4b9e98369ba8dcd22196a547c06222b601318722a93a4b5c47a27ae892f67cf67afc7e0774c4d16a89c672acafd325c8ff186e" },
                { "ro", "e7ea92f86eeb54bbdfe9e3d628553045bdbf39e5697acca4cb32ebcc351997aac5a039de4cf4778afff0b7caf5ae01d7ddd5b4b79801a4bb4e6eaf24f9fb3e93" },
                { "ru", "4a429dd4da3ea3475a92839b6bbda33f1742ba28f6b3cb1c5f15c8b6717c636ecae479fde2245f08a82ca124fb2b6d31eaf714c1e98873944411f07cff22537d" },
                { "sat", "3e809c5977d40a043e2e5bd29b6a5b077e38483e92da4601394b704c1c5139e92166b26bb5bac417b734de3691df2fbf943a9bc1a2a12b6e737feda3206f4975" },
                { "sc", "aab8db8e08312afb70d3b4771ed5ad074a4a8a95443eb02b8c32cb29caeb3f40d406ef58d83fa6c901fcad8cc8c3b44f4ed180667547e69963e7106275f3b55e" },
                { "sco", "787ee85404d380e9099005c322dd25c2321d4eb2f82e97f41026f439e4e082702e8a34956732a9eed0703008b74f86aaef0e7d1e14c122631ff82011390d7b37" },
                { "si", "c189c118af190c43ce9aeee6b008558d6c964d8882e806528685b2670664c3f641f069c174f64897b6adbbc227ea96611f4dd2c05e9804b9adc6f6e7635cff35" },
                { "sk", "8e069bfab3104b1bb241777a05b24bf263b7357eef9af373c023ba2ce277b41cde8501373668dcb32297143b79df576d240c1974040a0f6096f8d7194b66e88f" },
                { "skr", "85648c4d43c8d82e02dd5763ccc0fad74e7912450fe8f4fe7d87c98a2bd97200c107f1625f1213b68203f40dadc9b7890fb777437396d4c2b5e4416c330a6bc3" },
                { "sl", "bd2c710419b0acd07c4cf7df794e06b8c75771fc24eab77ca6b87e444df4562321bccbcf351f04ae897a583d38c8178df128b85973e9efb61c5fd98d5090246c" },
                { "son", "f67b6457b257b3f79bf602c18cc6a9e7712e28f15d5e6bd775e7a826bc51d336647dc52589e38047dc0a9d7f1a8fdd88384f4dd9fc2ffd5ded9de4a154765f53" },
                { "sq", "5e6925c12ccb6e42c424a04e5a30855b6c5932831fdee8edd2f9dbb6be7c4bd7acd39341a276d41dc0b89583a72b380fbe57dc032ace039eef61bd4cb18692c9" },
                { "sr", "bb1f059f2a0191e7e406c236cbb87c5d998c92f5722bd159916642fcd9633b125f530c581c7d5f47c098cdc6cbc9703d7ea776f01b4135dc5a435d4a986b2ea9" },
                { "sv-SE", "93365165c4ac8c15568b0e55156628442005ba9fbf45ac50b4f1eabf65a5cf502f0a6c3395a5a9afaecb01a3889802c553566634436ec2335c068b23bdffb43a" },
                { "szl", "adcf9b24ecaf617564a3165840b8679742e7169b869d4e87b8593c8c590271610caa40b6ffb97d62f035380288b2f15b59ceaa341bd04cb78d014050d9bdb013" },
                { "ta", "2664c4fc2d0d9c59bd5fb4fc3593df951a94c05e305be1d9870e884c4280cf71f675b58fa9e5a4ba5b649d4813dccc1aeee4151abcfd905056bb98f89ae0deac" },
                { "te", "b870b6b28126e2d254d6f3724125e0d458c790ab863dff65f2423afd609fb1523773b445c1936f908e00bcfa1e00b41cb7afd83ca2154d75324ef483182230f6" },
                { "tg", "f1557aa14e2f45a687428c3cb1f39004d6901e8397f64d01a1b61e86c26d1ce5400c00f5aae050f3316b54b0aa6816ceb2cbf19b52e5c973745f735f7c7f9fac" },
                { "th", "afcfb16d4f39ac6431c1e6980f4524cd89b5637c6901a5a39a90c50243ede0be1a62255c79e2b065f88664b2a7c07f6b338a52d5647abf8606a00eb23c5ea25c" },
                { "tl", "9a9f3cffc31ccefe479d965342670f1c73d188dfced200e215c90677996888adcb09e770f7b06cb6cba15ea78e5d20dd9025e93d8ba1d5731a23ee3ef7d35f40" },
                { "tr", "a1b232b234901d1946d7a3e17352935008277019a94aa91f467fc33ee8439e987c8eead1f84d43496512c62e927f32bf9ef9e51694468905a1306292d03bf183" },
                { "trs", "6a9bb16aed6bba216caf83cedadb1e552b7ddfa04f1165691b3f84a2c2f54408a6289d4935d8f67f511ea6d9493ba89faa25992ea528829ec0efffafac148192" },
                { "uk", "d38c66fc1db33efe83ad47588b771b24c8727d66934d5f9c11012a7fd43c73b66e867531c4b3509158ff7b58c09c33e311a7bbf731389ce35739bb5a4ddf4b89" },
                { "ur", "41538393742f23b1d219b44fac0476e295e73967fe3c6118bc8902c72ebb50f210ae9aa6be13cbf6199cce53014747a311632826143afa6918002c1b0fbd93b0" },
                { "uz", "fe8baa994be376806e1deecf7da67f0b8434d681f8ce782fc048bd088a8bbf9e0c9842c5e56523cb70703f6b03539cf0b12d41e840bac72dc9e059411095eac1" },
                { "vi", "2607827007db245ec997dedeeab334c0ba1f816f94eeaaa985318d4d1309a3b0c545b11befaecb537139c4fc120c301725a437c03caeadb29ee7735b24044949" },
                { "xh", "4448562eef1ed3c6964677f4347e3825c94d1437b35efede6fa482586b7435efd8ee4adcfe652993bbbdfe77ef17ddf56c1f4cccff7dcab02950f8bd9631118f" },
                { "zh-CN", "27b42af0b3281c31573b60f8be15db3a16e3a48d90d159a17fb458551b9b224fc61c0ad68ae81acfe2e960efb67c93296b2545860180f66cb790d3f95d5e732c" },
                { "zh-TW", "1a01699ecad1344de1b636bf1a4ca4872bc01804901a714a83fe9cb00ba8aadb5d4c6f99c890890b0231701426deca54bfd3f282763863778f0f7e3bf327c2b9" }
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
            const string knownVersion = "135.0";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                    );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return ["firefox", "firefox-" + languageCode.ToLower()];
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
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
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
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
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return [matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128]];
        }


        /// <summary>
        /// Determines whether the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Firefox...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // failure occurred
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
            return [];
        }


        /// <summary>
        /// language code for the Firefox ESR version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
